using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Example;
using TGC.Core.SceneLoader;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using Microsoft.DirectX.Direct3D;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;
using TGC.Core.PortalRendering;
using Microsoft.DirectX;

namespace TGC.Group.Model
{
    class CasaTenebrosa : TgcExample
    {
        private TgcScene scene;
        public CasaTenebrosa(string mediaDir, string shadersDir, TgcUserVars userVars, TgcModifiers modifiers) : base(mediaDir, shadersDir, userVars, modifiers)
        {
            Category = "Buildings";
            Name = "SpookyHouse";
            Description = "Modelo de mansion/casa tenebrosa como escenario de juego de Survival Horror.";
        }

        //mansion spencer 2.0.xml

        public override void Init()
        {

            var loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(MediaDir + "Desktop\\mansion spencer 2.0.xml");

            //Descactivar inicialmente a todos los modelos
            scene.setMeshesEnabled(false);

            //Camara = new TgcFpsCamera(new Vector3(0, 0, 0), 800f, 600f, Input);
           // Camara = new TgcCamera();

            var cameraPosition = new Vector3(0, 0, 125);
            var lookAt = Vector3.Empty;
            Camara.SetCamera(cameraPosition, lookAt);
            

            //Modifiers
            Modifiers.addBoolean("portalRendering", "PortalRendering", true);
            Modifiers.addBoolean("WireFrame", "WireFrame", false);
            Modifiers.addBoolean("showPortals", "Show Portals", false);

            //throw new NotImplementedException();
            UserVars.addVar("MeshCount");
        }

        public override void Update()
        {
            PreUpdate();
        }

        public override void Render()
        {
            PreRender();

            var enablePortalRendering = (bool)Modifiers["portalRendering"];
            if (enablePortalRendering)
            {
                //Actualizar visibilidad con PortalRendering
                scene.PortalRendering.updateVisibility(Camara.Position, Frustum);
            }
            else
            {
                //Habilitar todo todo
                scene.setMeshesEnabled(true);
            }

            //WireFrame
            var wireFrameEnable = (bool)Modifiers["WireFrame"];
            if (wireFrameEnable)
            {
                D3DDevice.Instance.Device.RenderState.FillMode = FillMode.WireFrame;
            }
            else
            {
                D3DDevice.Instance.Device.RenderState.FillMode = FillMode.Solid;
            }

            var meshCount = 0;
            //Renderizar modelos visibles, primero todos los modelos opacos (sin alpha)
            foreach (var mesh in scene.Meshes)
            {
                //Contador de modelos
                if (mesh.Enabled && !mesh.AlphaBlendEnable)
                {
                    meshCount++;
                }

                //Renderizar modelo y luego desactivarlo para el proximo cuadro
                mesh.render();
                mesh.Enabled = false;
            }

            //Luego renderizar modelos visibles con alpha
            foreach (var mesh in scene.Meshes)
            {
                //Contador de modelos
                if (mesh.Enabled && mesh.AlphaBlendEnable)
                {
                    meshCount++;
                }
                //Renderizar modelo y luego desactivarlo para el proximo cuadro
                mesh.render();
                mesh.Enabled = false;
            }

            //Contador de modelos visibles
            UserVars["MeshCount"] = meshCount.ToString();

            //Renderizar portales
            var showPortals = (bool)Modifiers["showPortals"];
            if (showPortals)
            {
                scene.PortalRendering.renderPortals();
            }

            PostRender();
        }


        public override void Dispose()
        {
            scene.disposeAll();
        }

    }
}
    

