using ZooWorld.Models;
using ZooWorld.Settings;
using ZooWorld.Views;

namespace ZooWorld.Services
{
    public sealed class AnimalPhysicsSetupService
    {
        public void Setup(AnimalView view, IAnimalModel model)
        {
            var movementSettings = model.MovementSettings;

            if (movementSettings is not FrogMovementSettings frogSettings)
            {

                return;
            }

            if (!frogSettings.SetPhysicMaterialNoBounce)
            {

                return;
            }

            view.Collider.material = frogSettings.FrogNoBounceMaterial;
        }
    }
}