using System.Threading.Tasks;

namespace H2DT.Management.Scenes
{
    public interface ISceneUnloadSubject
    {
        Task SceneUnloadingTask(SceneInfo sceneInfo);
    }
}
