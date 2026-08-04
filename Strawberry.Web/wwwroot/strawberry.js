// strawberry.js — Strawberry Game Engine web bootstrap module
// Ships with the Strawberry.Web NuGet package. Game projects import this
// from their index.html; they do not need to write their own bootstrap JS.

import { dotnet } from './_framework/dotnet.js';

// ── Internal state ───────────────────────────────────────────

let canvas = null;
let interop = null;
let onReadyCallback = null;

let DB_NAME = "strawberry_user_data";
const STORE_NAME = "files";
let db = null;
let dpr = 1;

// ── IndexedDB helpers (internal) ─────────────────────────────

function openDB() {
    return new Promise((resolve, reject) => {
        if (db) { resolve(db); return; }
        const req = indexedDB.open(DB_NAME, 1);
        req.onupgradeneeded = (e) => {
            const d = e.target.result;
            if (!d.objectStoreNames.contains(STORE_NAME)) {
                d.createObjectStore(STORE_NAME);
            }
        };
        req.onsuccess = (e) => { db = e.target.result; resolve(db); };
        req.onerror = () => reject(req.error);
    });
}

function arrayBufferToBase64(buffer) {
    let binary = '';
    const bytes = new Uint8Array(buffer);
    const chunk = 0x8000;
    for (let i = 0; i < bytes.length; i += chunk) {
        binary += String.fromCharCode.apply(null, bytes.subarray(i, i + chunk));
    }
    return btoa(binary);
}

function base64ToArrayBuffer(base64) {
    const binary = atob(base64);
    const bytes = new Uint8Array(binary.length);
    for (let i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return bytes.buffer;
}

async function loadAll() {
    const d = await openDB();
    return new Promise((resolve, reject) => {
        const tx = d.transaction(STORE_NAME, "readonly");
        const store = tx.objectStore(STORE_NAME);
        const result = {};
        const req = store.openCursor();
        req.onsuccess = (e) => {
            const cursor = e.target.result;
            if (cursor) {
                const val = cursor.value;
                if (val instanceof ArrayBuffer) {
                    result[cursor.key] = arrayBufferToBase64(val);
                } else if (val instanceof Uint8Array) {
                    result[cursor.key] = arrayBufferToBase64(val.buffer);
                }
                cursor.continue();
            } else {
                resolve(JSON.stringify(result));
            }
        };
        req.onerror = () => reject(req.error);
    });
}

// ── Canvas resize (internal) ─────────────────────────────────

function setupResizeHandler() {
    function resizeCanvas() {
        dpr = window.devicePixelRatio || 1;
        const w = Math.floor(canvas.clientWidth * dpr);
        const h = Math.floor(canvas.clientHeight * dpr);

        if (canvas.width !== w || canvas.height !== h) {
            canvas.width = w;
            canvas.height = h;
            interop.OnCanvasResize(canvas.clientWidth * dpr, canvas.clientHeight * dpr, dpr);
        }
    }

    resizeCanvas();
    new ResizeObserver(resizeCanvas).observe(canvas);
}

// ── Lifecycle handlers (internal) ────────────────────────────

function setupLifecycleHandlers() {
    document.addEventListener('visibilitychange', () => {
        if (document.hidden) interop.OnPause();
        else interop.OnResume();
    });

    canvas.addEventListener("webglcontextlost", (e) => {
        e.preventDefault();
        console.warn("WebGL context lost");
        interop.OnGraphicsContextLost();
    }, false);

    canvas.addEventListener("webglcontextrestored", () => {
        console.log("WebGL context restored");
        interop.OnGraphicsContextRestored();
    }, false);

    window.addEventListener('blur', () => interop.OnFocusLost());
    window.addEventListener('focus', () => interop.OnFocusGained());
}

// ── Input handlers (internal) ────────────────────────────────

function setupInputHandlers() {
    canvas.addEventListener("keydown", (e) => {
        e.stopPropagation();
        interop.OnKeyDown(e.shiftKey, e.ctrlKey, e.altKey, e.repeat, e.keyCode);
    }, false);

    canvas.addEventListener("keyup", (e) => {
        e.stopPropagation();
        interop.OnKeyUp(e.shiftKey, e.ctrlKey, e.altKey, e.keyCode);
    }, false);

    canvas.addEventListener("mousemove", (e) => {
        interop.OnMouseMove(0, e.offsetX * dpr, e.offsetY * dpr);
    }, false);

    canvas.addEventListener("mousedown", (e) => {
        interop.OnMouseDown(0, e.shiftKey, e.ctrlKey, e.altKey, e.button);
    }, false);

    canvas.addEventListener("mouseup", (e) => {
        interop.OnMouseUp(0, e.shiftKey, e.ctrlKey, e.altKey, e.button);
    }, false);

    // Touch
    const shouldIgnore = (e) => {
        e.preventDefault();
        return e.touches.length > 1 || (e.type === "touchend" && e.touches.length > 0);
    };

    const getTouchPos = (e) => {
        const touch = e.changedTouches[0];
        const bcr = e.target.getBoundingClientRect();
        return { x: (touch.clientX - bcr.x) * dpr, y: (touch.clientY - bcr.y) * dpr };
    };

    canvas.addEventListener("touchstart", (e) => {
        if (shouldIgnore(e)) return;
        const p = getTouchPos(e);
        interop.OnMouseMove(0, p.x, p.y);
        interop.OnMouseDown(0, e.shiftKey, e.ctrlKey, e.altKey, 0);
    }, false);

    canvas.addEventListener("touchmove", (e) => {
        if (shouldIgnore(e)) return;
        const p = getTouchPos(e);
        interop.OnMouseMove(0, p.x, p.y);
    }, false);

    canvas.addEventListener("touchend", (e) => {
        if (shouldIgnore(e)) return;
        const p = getTouchPos(e);
        interop.OnMouseMove(0, p.x, p.y);
        interop.OnMouseUp(0, e.shiftKey, e.ctrlKey, e.altKey, 0);
    }, false);
}

// strawberry.js — Strawberry Game Engine web bootstrap module

function getModuleImports(getAssemblyExports) {
    return {
        request_root_url: () => window.location.toString(),
        get_canvas_width: () => canvas.width,
        get_canvas_height: () => canvas.height,

        initialize: async () => {
            if (interop == null) {
                const exports = await getAssemblyExports('Strawberry.Web');
                interop = exports.Strawberry.Web.Interop;
            }
            try {
                const json = await loadAll();
                interop.SetUserDataCache(json);
            } catch (e) {
                console.error("Failed to load user data:", e);
                interop.SetUserDataCache("{}");
            }
            canvas.focus();
            setupLifecycleHandlers();
            setupResizeHandler();
            setupInputHandlers();

            // Notify the developer that initialization is complete.
            // They can use this to hide a loading screen, start audio, etc.
            if (onReadyCallback) {
                try {
                    onReadyCallback();
                } catch (e) {
                    console.error("Strawberry onReady callback threw:", e);
                }
            }
        },

        set_game_name: (name) => {
            const safe = name.replace(/[^a-zA-Z0-9_-]/g, '_');
            const newName = "strawberry_" + safe;
            if (newName !== DB_NAME) { DB_NAME = newName; db = null; }
        },

        storage: {
            // ... [unchanged] ...
        }
    };
}

// ── Public API ───────────────────────────────────────────────

/**
 * Starts the Strawberry game engine.
 *
 * @param {Object} [options={}] - Startup options.
 * @param {HTMLCanvasElement|string} [options.canvas] - Canvas element or selector. Defaults to #canvas.
 * @param {Function} [options.onReady] - Called after the engine initializes (user data loaded, handlers wired).
 *        Use this to hide a loading screen, start audio, transition UI, etc.
 * @returns {Promise<void>}
 *
 * @example
 * import { start } from './strawberry.js';
 * start({
 *     onReady: () => {
 *         document.getElementById('loading')?.remove();
 *     }
 * });
 */
export async function start(options = {}) {
    // Resolve canvas
    if (options.canvas instanceof HTMLCanvasElement)
        canvas = options.canvas;
    else if (typeof options.canvas === 'string')
        canvas = document.querySelector(options.canvas);
    else
        canvas = document.getElementById('canvas');
    if (!canvas)
        throw new Error('Strawberry: canvas not found. Provide <canvas id="canvas"> or pass { canvas }.');

    // Store the ready callback — invoked from initialize() after engine setup
    onReadyCallback = options.onReady || null;

    // Create runtime and register JS imports
    const { setModuleImports, getAssemblyExports } = await dotnet
        .withDiagnosticTracing(false)
        .withApplicationArgumentsFromQuery()
        .create();

    // Attach canvas to .NET WASM module
    dotnet.instance.Module["canvas"] = canvas;

    setModuleImports("strawberry.js", getModuleImports(getAssemblyExports));

    // Start the .NET runtime
    await dotnet.run();
}