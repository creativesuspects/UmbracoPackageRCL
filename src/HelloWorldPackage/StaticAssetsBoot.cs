using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;

namespace HelloWorldPackage.Assets
{
    public class StaticAssetsBoot : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddPackageStaticAssets();
        }
    }

    public static class PackageStaticAssetsExtensions
    {
        public static IUmbracoBuilder AddPackageStaticAssets(this IUmbracoBuilder builder)
        {
            // don't add if the filter is already there .
            if (builder.ManifestFilters().Has<PackageAssetManifestFilter>())
                return builder;

            // add the package manifest programatically. 
            builder.ManifestFilters().Append<PackageAssetManifestFilter>();

            return builder;
        }
    }

    internal class PackageAssetManifestFilter : IManifestFilter
    {
        private readonly IManifestParser parser;

        public PackageAssetManifestFilter(IManifestParser parser)
        {
            this.parser = parser;
        }

        void IManifestFilter.Filter(List<PackageManifest> manifests)
        {
            var manifestJson = PackageAssetManifestReader.ReadManifest();
            var manifest = parser.ParseManifest(manifestJson);
            manifest.PackageName = "Hello World Package";

            manifests.Add(manifest);
        }
    }

    internal class PackageAssetManifestReader
    {
        public static string ReadManifest()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using Stream stream = assembly.GetManifestResourceStream("HelloWorldPackage.package.manifest")!;
            using StreamReader reader = new(stream);
            string result = reader.ReadToEnd();

            return result;
        }
    }
}
