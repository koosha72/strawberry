import { dotnet } from './_framework/dotnet.js'

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const config = getConfig();
console.log(config);

const exports = await getAssemblyExports(config.mainAssemblyName);

var canvas = globalThis.document.getElementById("canvas");
dotnet.instance.Module["canvas"] = canvas;

setModuleImports("main.js", {
    initialize: () => {
        console.log('initialize');
    }
});


await dotnet.run();