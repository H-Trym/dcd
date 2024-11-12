using api.Dtos;
using api.Exceptions;
using api.Models;
using api.StartupConfiguration;

using AutoMapper;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


public class BlobStorageService(BlobServiceClient blobServiceClient,
    IImageRepository imageRepository,
    IConfiguration configuration,
    IMapper mapper)
    : IBlobStorageService
{
    private readonly string _containerName = GetContainerName(configuration);

    private static string GetContainerName(IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("AppConfiguration__Environment") ?? "default";
        var containerKey = environment switch
        {
            DcdEnvironments.LocalDev => "AzureStorageAccountImageContainerCI",
            DcdEnvironments.Ci => "AzureStorageAccountImageContainerCI",
            DcdEnvironments.RadixDev => "AzureStorageAccountImageContainerCI",
            DcdEnvironments.Dev => "AzureStorageAccountImageContainerCI",
            DcdEnvironments.Qa => "AzureStorageAccountImageContainerQA",
            DcdEnvironments.RadixQa => "AzureStorageAccountImageContainerQA",
            DcdEnvironments.Prod => "AzureStorageAccountImageContainerProd",
            DcdEnvironments.RadixProd => "AzureStorageAccountImageContainerProd",
            _ => throw new InvalidOperationException($"Unknown fusion environment: {environment}")
        };

        return configuration[containerKey]
                             ?? throw new InvalidOperationException($"Container name configuration for {environment} is missing.");
    }

    private static string SanitizeBlobName(string name)
    {
        return name.Replace(" ", "-").Replace("/", "-").Replace("\\", "-");
    }

    public async Task<ImageDto> SaveImage(Guid projectId, string projectName, IFormFile image, Guid? caseId = null)
    {
        var sanitizedProjectName = SanitizeBlobName(projectName);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        var imageId = Guid.NewGuid();

        if (projectId == Guid.Empty || caseId == Guid.Empty)
        {
            throw new ArgumentException("ProjectId and/or CaseId cannot be empty.");
        }

        var blobName = caseId.HasValue
            ? $"{sanitizedProjectName}/cases/{caseId}/{imageId}"
            : $"{sanitizedProjectName}/projects/{projectId}/{imageId}";

        var blobClient = containerClient.GetBlobClient(blobName);

        await using var stream = image.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = image.ContentType });

        var imageUrl = blobClient.Uri.ToString();
        var createTime = DateTimeOffset.UtcNow;

        var imageEntity = new Image
        {
            Id = imageId,
            Url = imageUrl,
            CreateTime = createTime,
            CaseId = caseId,
            ProjectId = projectId,
            ProjectName = sanitizedProjectName
        };

        await imageRepository.AddImage(imageEntity);

        var imageDto = mapper.Map<ImageDto>(imageEntity);

        if (imageDto == null)
        {
            throw new InvalidOperationException("Image mapping failed.");
        }

        return imageDto;
    }

    public async Task<List<ImageDto>> GetCaseImages(Guid caseId)
    {
        var images = await imageRepository.GetImagesByCaseId(caseId);

        var imageDtos = mapper.Map<List<ImageDto>>(images);

        if (imageDtos == null)
        {
            throw new InvalidOperationException("Image mapping failed.");
        }
        return imageDtos;
    }

    public async Task<List<ImageDto>> GetProjectImages(Guid projectId)
    {
        var images = await imageRepository.GetImagesByProjectId(projectId);

        var imageDtos = mapper.Map<List<ImageDto>>(images);

        if (imageDtos == null)
        {
            throw new InvalidOperationException("Image mapping failed.");
        }
        return imageDtos;
    }

    public async Task DeleteImage(Guid imageId)
    {
        var image = await imageRepository.GetImageById(imageId);
        if (image == null)
        {
            throw new NotFoundInDBException("Image not found.");
        }

        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        var sanitizedProjectName = SanitizeBlobName(image.ProjectName);
        var blobName = image.CaseId.HasValue
            ? $"{sanitizedProjectName}/cases/{image.CaseId}/{image.Id}"
            : $"{sanitizedProjectName}/projects/{image.ProjectId}/{image.Id}";

        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync();
        await imageRepository.DeleteImage(image);
    }
}
