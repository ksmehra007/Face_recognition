﻿using Autofac;
using Mallenom;
using Recognizer.Database;

using Mallenom.Video;
using Recognizer.Configuration;
using Mallenom.AppServices;

namespace Recognizer
{
	public static class RegistrationServices
	{
		public static IContainer CreateContainer(AppCriticalServices appCriticalServices)
		{
			Verify.Argument.IsNotNull(appCriticalServices, nameof(appCriticalServices));
			
			var containerBuilder = new ContainerBuilder();

			appCriticalServices.Register(containerBuilder);

			containerBuilder
				.RegisterType<AppBootstrapper>()
				.SingleInstance()
				.AsSelf();

			containerBuilder
				.RegisterType<DatabaseService>()
				.SingleInstance()
				.AsSelf();

			var videoSourceProvider = new IPCameraSourceProvider();
			var configuration = videoSourceProvider.TryGetConfiguration();
			var videoSource = videoSourceProvider.CreateVideoSource();

			containerBuilder
				.RegisterInstance(videoSourceProvider)
				.As<IVideoSourceProvider>();

			containerBuilder
				.RegisterInstance(new VideoSourceConfiguration(videoSource, configuration))
				.As<ISerializableConfiguration>();

			containerBuilder
				.RegisterInstance(videoSource)
				.As<IVideoSource>();

			containerBuilder.RegisterModule<ConfigurationModule>();
			containerBuilder.RegisterModule<DatabaseModule>();

			return containerBuilder.Build();
		}
	}
}
