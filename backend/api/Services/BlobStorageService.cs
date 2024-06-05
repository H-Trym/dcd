using api.Dtos;
using api.Models;

using AutoMapper;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IImageRepository _imageRepository;
    private readonly IMapper _mapper;
    private readonly string _containerName;

    public BlobStorageService(BlobServiceClient blobServiceClient, IImageRepository imageRepository, IConfiguration configuration, IMapper mapper)
    {
        _blobServiceClient = blobServiceClient;
        _imageRepository = imageRepository;
        _mapper = mapper;
        _containerName = configuration["azureStorageAccountImageContainerName"]
                         ?? throw new InvalidOperationException("Container name configuration is missing.");

        if (string.IsNullOrEmpty(_containerName))
        {
            throw new InvalidOperationException("Container name configuration is missing or empty.");
        }
    }
    private string SanitizeBlobName(string name)
    {
        return name.Replace(" ", "-").Replace("/", "-").Replace("\\", "-");
    }
    public async Task<ImageDto> SaveImage(Guid projectId, string projectName, IFormFile image, Guid caseId)
    {
        var sanitizedProjectName = SanitizeBlobName(projectName);
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        var imageId = Guid.NewGuid();
        var blobName = $"{sanitizedProjectName}/{caseId}/{imageId}";
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

        await _imageRepository.AddImage(imageEntity);

        var imageDto = _mapper.Map<ImageDto>(imageEntity);

        if (imageDto == null)
        {
            throw new InvalidOperationException("Image mapping failed.");
        }

        return imageDto;
    }

    public async Task<List<ImageDto>> GetCaseImages(Guid caseId)
    {
        var images = await _imageRepository.GetImagesByCaseId(caseId);

        var imageDtos = _mapper.Map<List<ImageDto>>(images);

        if (imageDtos == null)
        {
            throw new InvalidOperationException("Image mapping failed.");
        }
        return imageDtos;
    }
}
