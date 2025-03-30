import { dotnet } from './_framework/dotnet.js'

globalThis.document.getElementById("start").addEventListener('click', function () {
    globalThis.document.getElementById("start").remove();
    play();
}, false);

async function play() {
    var loading = globalThis.document.getElementById("loading");
    loading.style.display = 'block';
    const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
        .withDiagnosticTracing(false)
        .withApplicationArgumentsFromQuery()
        .create();

    var interop = null;

    const config = getConfig();

    var canvas = globalThis.document.getElementById("canvas");
    dotnet.instance.Module["canvas"] = canvas;

    setModuleImports("main.js", {
        request_root_url: () => {
            return window.location.toString();
        },
        initialize: async () => {
            loading.remove();
            if (interop == null) {
                const exports = await getAssemblyExports('Strawberry.Web');
                interop = exports.Strawberry.Web.Interop;
            }

            var keyDown = (e) => {
                e.stopPropagation();
                var shift = e.shiftKey;
                var ctrl = e.ctrlKey;
                var alt = e.altKey;
                var repeat = e.repeat;
                var code = e.keyCode;

                interop.OnKeyDown(shift, ctrl, alt, repeat, code);
            }

            var keyUp = (e) => {
                e.stopPropagation();
                var shift = e.shiftKey;
                var ctrl = e.ctrlKey;
                var alt = e.altKey;
                var code = e.keyCode;

                interop.OnKeyUp(shift, ctrl, alt, code);
            }

            var mouseMove = (e) => {
                var x = e.offsetX;
                var y = e.offsetY;
                interop.OnMouseMove(0, x, y);
            }

            var mouseDown = (e) => {
                var shift = e.shiftKey;
                var ctrl = e.ctrlKey;
                var alt = e.altKey;
                var button = e.button;

                interop.OnMouseDown(0, shift, ctrl, alt, button);
            }

            var mouseUp = (e) => {
                var shift = e.shiftKey;
                var ctrl = e.ctrlKey;
                var alt = e.altKey;
                var button = e.button;

                interop.OnMouseUp(0, shift, ctrl, alt, button);
            }

            var shouldIgnore = (e) => {
                e.preventDefault();
                return e.touches.length > 1 || e.type == "touchend" && e.touches.length > 0;
            }

            var touchStart = (e) => {
                if (shouldIgnore(e))
                    return;

                var shift = e.shiftKey;
                var ctrl = e.ctrlKey;
                var alt = e.altKey;
                var button = 0;
                var touch = e.changedTouches[0];
                var bcr = e.target.getBoundingClientRect();
                var x = touch.clientX - bcr.x;
                var y = touch.clientY - bcr.y;

                interop.OnMouseMove(0, x, y);
                interop.OnMouseDown(0, shift, ctrl, alt, button);
            }

            var touchMove = (e) => {
                if (shouldIgnore(e))
                    return;

                var touch = e.changedTouches[0];
                var bcr = e.target.getBoundingClientRect();
                var x = touch.clientX - bcr.x;
                var y = touch.clientY - bcr.y;

                interop.OnMouseMove(0, x, y);
            }

            var touchEnd = (e) => {
                if (shouldIgnore(e))
                    return;

                var shift = e.shiftKey;
                var ctrl = e.ctrlKey;
                var alt = e.altKey;
                var button = 0;
                var touch = e.changedTouches[0];
                var bcr = e.target.getBoundingClientRect();
                var x = touch.clientX - bcr.x;
                var y = touch.clientY - bcr.y;

                interop.OnMouseMove(0, x, y);
                interop.OnMouseUp(0, shift, ctrl, alt, button);
            }

            canvas.addEventListener("keydown", keyDown, false);
            canvas.addEventListener("keyup", keyUp, false);
            canvas.addEventListener("mousemove", mouseMove, false);
            canvas.addEventListener("mousedown", mouseDown, false);
            canvas.addEventListener("mouseup", mouseUp, false);
            canvas.addEventListener("touchstart", touchStart, false);
            canvas.addEventListener("touchmove", touchMove, false);
            canvas.addEventListener("touchend", touchEnd, false);

            canvas.tabIndex = 1000;
        }
    });


    await dotnet.run();
}