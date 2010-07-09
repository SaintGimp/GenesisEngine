using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap.Configuration.DSL;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using StructureMap;
using StructureMap.TypeRules;

namespace GenesisEngine
{
    public class GenesisRegistry : Registry
    {
        public GenesisRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
                
                // These are excluded because they have contructor dependencies that are satisfied by
                // factories, aren't pulled from the container, and we don't want them checked in unit tests.
                x.ExcludeType<QuadNode>();
                x.ExcludeType<QuadMesh>();
                x.ExcludeType<QuadMeshRenderer>();
            });

            MakeSingleton<Genesis>();
            MakeSingleton<ICamera>();
            MakeSingleton<ICameraController>();
            MakeSingleton<IEventAggregator>();
            MakeSingleton<IInputMapper>();
            MakeSingleton<ISettings>();
            MakeSingleton<Statistics>();
            MakeSingleton<MainPresenter>();

            For<IScreenCustodian<SettingsView, SettingsViewModel>>().Use<ScreenCustodian<SettingsView, SettingsViewModel>>();
            For<IScreenCustodian<StatisticsView, StatisticsViewModel>>().Use<ScreenCustodian<StatisticsView, StatisticsViewModel>>();
            
            For<ContentManager>().Use(x => ObjectFactory.GetInstance<Genesis>().Content);
            For<GraphicsDevice>().Use(x => ObjectFactory.GetInstance<Genesis>().GraphicsDeviceManager.GraphicsDevice);
            For<IInputState>().Use<XnaInputState>();
            For<ITerrainColorizer>().Use(x => new EdgeColorizer(ObjectFactory.GetInstance<TerrainColorizer>(), ObjectFactory.GetInstance<ISettings>()));

            For<IHeightfieldGenerator>().Use<LayeredHeightfieldGenerator>();
            RegisterInterceptor(new EventAggregatorTypeInterceptor());
        }

        private void MakeSingleton<T>()
        {
            For<T>().Singleton();

            if (typeof(T).IsConcrete())
            {
                For<T>().Use<T>();
            }
        }
    }
}
