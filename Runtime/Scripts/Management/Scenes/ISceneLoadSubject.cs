using System.Threading.Tasks;

namespace H2DT.Management.Scenes
{
    public interface ISceneLoadSubject
    {
        Task SceneLoadingTask(SceneInfo sceneInfo);
    }
}
