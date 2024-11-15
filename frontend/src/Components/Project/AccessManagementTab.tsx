import { useEffect, useState } from "react"
import {
    Button,
    Icon,
    Typography,
} from "@equinor/eds-core-react"
import { PersonSelect, PersonListItem, PersonSelectEvent } from "@equinor/fusion-react-person"
import Grid from "@mui/material/Grid"
import { useQuery, useQueryClient } from "@tanstack/react-query"
import styled from "styled-components"
import {
    edit,
    visibility,
    info_circle,
    chevron_down,
    chevron_up,
    external_link,
} from "@equinor/eds-icons"
import { useMediaQuery } from "@mui/material"
import { useModuleCurrentContext } from "@equinor/fusion-framework-react-module-context"

import { projectQueryFn } from "@/Services/QueryFunctions"
import { useAppContext } from "@/Context/AppContext"
import { FusionPersonV1, UserRole } from "@/Models/AccessManagement"
import { GetProjectMembersService } from "@/Services/ProjectMembersService"
import { useProjectContext } from "@/Context/ProjectContext"
import PersonMock from "./Components/PersonMock"
import { GetProjectService } from "@/Services/ProjectService"

const PeopleMock = {
    orgChart: [
        {
            name: "Geir Nordmann",
            email: "geir.nordmann@equinor.com",
        },
        {
            name: "Egil Nordmann",
            email: "Egil.nordmann@equinor.com",
        },
        {
            name: "Erling Nordmann",
            email: "erling.nordmann@equinor.com",
        },
        {
            name: "Goggen Nordmann",
            email: "goggen.nordmann@equinor.com",
        },
    ],
}

const EditorViewerContainer = styled(Grid)<{ $isSmallScreen: boolean }>`
    display: flex;
    justify-content: center;
    padding: 15px;
    margin-top: 35px;
    flex-direction: ${(props) => (props.$isSmallScreen ? "column" : "row")}!important;
`

const EditorViewerContent = styled.div<{ $right?: boolean; $isSmallScreen?: boolean; }>`
    display: flex;
    flex-direction: column;
    width: 100%;
    margin: ${(props) => (props.$right ? "0 0 0 50px" : "0 50px 0 0")};
    margin: ${(props) => (props.$isSmallScreen && "0")};
`

const EditorViewerHeading = styled.div<{ $smallGap?: boolean; }>`
    display: flex;
    align-items: center;
    width: 100%;
    gap: ${(props) => (props.$smallGap ? "3px" : "10px")};
    margin-bottom: 15px;
`

const PeopleContainer = styled.div`
    display: flex;
    margin: 20px 0 40px 0;
    flex-direction: column;
    gap: 20px;
`

const ClickableHeading = styled(Grid)`
    display: flex;
    align-items: center;
    margin-top: 35px;
    gap: 10px;
    cursor: pointer;
`

const AccessManagementTab = () => {
    const { editMode } = useAppContext()
    const { projectId, setAccessRights, accessRights } = useProjectContext()
    const queryClient = useQueryClient()
    const isSmallScreen = useMediaQuery("(max-width:960px)", { noSsr: true })
    const { setSnackBarMessage } = useAppContext()
    const { currentContext } = useModuleCurrentContext()
    const [expandAllAccess, setExpandAllAccess] = useState<boolean>(true)
    const [orgChartPeople, setOrgChartPeople] = useState<FusionPersonV1[] | null>(null)

    const { data: projectApiData } = useQuery({
        queryKey: ["projectApiData", projectId],
        queryFn: () => projectQueryFn(projectId),
        enabled: !!projectId,
    })
    // Hvem skal kunne fjerne personer fra prosjektet? sjekker foreløpig kun på editMode

    const handleRemovePerson = async (userId: string) => {
        await (await GetProjectMembersService()).deletePerson(projectId, userId).then(() => {
            queryClient.invalidateQueries(
                { queryKey: ["projectApiData", projectId] },
            )
        })
    }

    const handleAddPerson = async (e: PersonSelectEvent, role: UserRole) => {
        const personToAdd = e.target.controllers.element.listItems[0].azureUniqueId
        // return null if person is allready in it (to win it), can it be disabled in another way?
        // det må finnes en måte å cleare ut selected person, kan ikke tro noe annet
        if ((!personToAdd && !projectId) || projectApiData?.projectMembers.some((p) => p.userId === personToAdd)) { return null }

        const addPerson = await (await GetProjectMembersService()).addPerson(projectId, { UserId: personToAdd, Role: role })
        if (addPerson) {
            queryClient.invalidateQueries(
                { queryKey: ["projectApiData", projectId] },
            )
        }
        return null
    }

    useEffect(() => {
        const fetchOrgChartPeople = async () => {
            if (!currentContext?.id) {
                return
            }
            const projectMembersService = await GetProjectMembersService()
            try {
                const peopleToAdd = await projectMembersService.getOrgChartPeople(currentContext.id)
                console.log(peopleToAdd)
                setOrgChartPeople(peopleToAdd)
            } catch (error) {
                console.log(error)
                setSnackBarMessage("A problem occured getting user access")
            }
        }
        fetchOrgChartPeople()
    }, [currentContext?.id])

    useEffect(() => {
        const fetchAccess = async () => {
            if (!projectId) {
                return
            }
            const projectService = await GetProjectService()
            try {
                const access = await projectService.getAccess(projectId)
                setAccessRights(access)
            } catch (error) {
                // hvorfor får vi error her??? externalId??
                console.log(error)
                setSnackBarMessage("A problem occured getting user access")
            }
        }
        fetchAccess()
    }, [projectId])

    // Lag skeletons for loading state, når vi får persondata fra api
    if (!projectApiData) {
        return <div>Loading project data...</div>
    }

    return (
        <Grid container direction="column" paddingX="10px" spacing={2}>
            <Grid item>
                <Typography variant="h3">Access Management</Typography>
            </Grid>
            <Grid item>
                <Typography variant="body_short">
                    On this page the project admins can add and remove members to the project.
                    If the project classification is set to “restricted” or “confidential”,
                    only the project members and the application admin can access it.
                    Project members from Org chart with “PMT” are automatically added as project editors after DG0. External users can also be added here.
                </Typography>
            </Grid>
            <EditorViewerContainer $isSmallScreen={isSmallScreen}>
                <EditorViewerContent>
                    <EditorViewerHeading>
                        <Icon data={edit} />
                        <Typography variant="h6">Project editors</Typography>
                    </EditorViewerHeading>
                    {editMode && accessRights?.canEdit && (
                        <PersonSelect
                            placeholder="Add new"
                            onSelect={(selectedPerson) => handleAddPerson(selectedPerson as PersonSelectEvent, UserRole.Editor)}
                        />
                    )}
                    <PeopleContainer>
                        {projectApiData?.projectMembers?.filter((m) => m.role === 1).map((person) => (
                            <PersonListItem key={person.userId} azureId={person.userId}>
                                {editMode && accessRights?.canEdit && <Button variant="ghost" color="danger" onClick={() => handleRemovePerson(person.userId)}>Remove</Button> }
                            </PersonListItem>
                        ))}
                    </PeopleContainer>
                    <Typography variant="h6">PMT members from the project orgchart:</Typography>
                    <PeopleContainer>
                        {PeopleMock.orgChart.map((person) => (
                            <PersonMock
                                key={person.email}
                                name={person.name}
                                email={person.email}
                                hideAction
                            />
                        ))}
                    </PeopleContainer>
                </EditorViewerContent>
                <hr />
                <EditorViewerContent $right $isSmallScreen={isSmallScreen}>
                    <EditorViewerHeading>
                        <Icon data={visibility} />
                        <Typography variant="h6">Project viewers</Typography>
                    </EditorViewerHeading>
                    {editMode && accessRights?.canEdit && (
                        <PersonSelect
                            placeholder="Add new"
                            onSelect={(selectedPerson) => handleAddPerson(selectedPerson as PersonSelectEvent, UserRole.Viewer)}
                        />
                    )}
                    <PeopleContainer>
                        {projectApiData?.projectMembers?.filter((m) => m.role === 0).map((person) => (
                            <PersonListItem key={person.userId} azureId={person.userId}>
                                {editMode && accessRights?.canEdit && <Button variant="ghost" color="danger" onClick={() => handleRemovePerson(person.userId)}>Remove</Button> }
                            </PersonListItem>
                        ))}
                    </PeopleContainer>
                </EditorViewerContent>
            </EditorViewerContainer>
            <ClickableHeading item onClick={() => setExpandAllAccess(!expandAllAccess)}>
                <Icon data={info_circle} />
                <Typography variant="h4">Would you like to access all internal Concept App projects?</Typography>
                <Icon data={expandAllAccess ? chevron_up : chevron_down} />
            </ClickableHeading>
            {expandAllAccess
                && (
                    <Grid container item>
                        <Typography variant="body_short">
                            In order to access all internal projects in Concept App, you need to apply for access in one of the AccessIT groups listed below.
                            Keep in mind that “Restricted” or “Confidential” projects are only accesible to project members.
                        </Typography>
                        <Grid container item gap="100px" marginTop="25px">
                            <Grid item>
                                <EditorViewerHeading $smallGap>
                                    <Icon data={edit} />
                                    <Typography variant="h6">Application editor</Typography>
                                </EditorViewerHeading>
                                <EditorViewerHeading $smallGap>
                                    <Typography
                                        link
                                        href="https://accessit.equinor.com/Search/Search?term=Fusion+-+Concept+App+-+Project+Member+%28FUSION%29"
                                        target="_blank"
                                    >
                                        Concept App - Editor
                                    </Typography>
                                    <Icon color="#007079" size={16} data={external_link} />
                                </EditorViewerHeading>
                            </Grid>
                            <Grid item>
                                <EditorViewerHeading>
                                    <Typography variant="h6">Application admin</Typography>
                                </EditorViewerHeading>
                                <EditorViewerHeading $smallGap>
                                    <Typography
                                        link
                                        href="https://accessit.equinor.com/Search/Search?term=Fusion+-+Concept+App+-+Admin+%28FUSION%29"
                                        target="_blank"
                                    >
                                        Concept App - Admin
                                    </Typography>
                                    <Icon color="#007079" size={16} data={external_link} />
                                </EditorViewerHeading>
                            </Grid>
                        </Grid>
                        <Grid item marginTop="20px">
                            <EditorViewerHeading>
                                <Icon data={visibility} />
                                <Typography variant="h6">Application viewers</Typography>
                            </EditorViewerHeading>
                            <EditorViewerHeading $smallGap>
                                <Typography
                                    link
                                    href="https://accessit.equinor.com/Search/Search?term=Fusion+-+Concept+App+-+Observer+%28FUSION%29"
                                    target="_blank"
                                >
                                    Concept App - Viewer
                                </Typography>
                                <Icon color="#007079" size={16} data={external_link} />
                            </EditorViewerHeading>
                            <Grid item marginTop="50px" display="flex" flexDirection="column" gap="15px">
                                <Typography variant="body_short">
                                    The following AccessIT groups also have view access in the app:
                                </Typography>
                                <EditorViewerHeading $smallGap>
                                    <Typography
                                        link
                                        href="https://accessit.equinor.com/Search/Search?term=Chief+Engineers+%28FUSION%29"
                                        target="_blank"
                                    >
                                        Chief Engineers (FUSION)
                                    </Typography>
                                    <Icon color="#007079" size={16} data={external_link} />
                                </EditorViewerHeading>
                                <EditorViewerHeading $smallGap>
                                    <Typography
                                        link
                                        href="https://accessit.equinor.com/Search/Search?term=Leading+Advisors+Project+Management+%26+Control+%28FUSION%29"
                                        target="_blank"
                                    >
                                        Leading Advisors Project Management & Control (FUSION)
                                    </Typography>
                                    <Icon color="#007079" size={16} data={external_link} />
                                </EditorViewerHeading>
                                <EditorViewerHeading $smallGap>
                                    <Typography
                                        link
                                        href="https://accessit.equinor.com/Search/Search?term=Project+Development+Center+-+Management+%28FUSION%29"
                                        target="_blank"
                                    >
                                        Project Development Center - Management (FUSION)
                                    </Typography>
                                    <Icon color="#007079" size={16} data={external_link} />
                                </EditorViewerHeading>
                            </Grid>
                        </Grid>
                    </Grid>
                )}
        </Grid>
    )
}

export default AccessManagementTab