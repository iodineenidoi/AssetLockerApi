namespace AssetLockerApi.Core;

public class ProjectsController
{
    private Dictionary<string, Project> _projects;

    public ProjectsController()
    {
        _projects = new Dictionary<string, Project>();
    }

    public Project GetProject(string projectName)
    {
        if (!_projects.ContainsKey(projectName))
            _projects[projectName] = new Project();

        return _projects[projectName];
    }
}