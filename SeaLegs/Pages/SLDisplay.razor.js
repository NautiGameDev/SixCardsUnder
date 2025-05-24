
//Browser window sizing and resizing

let gameCanvas;
let gameContext;
let displayCanvas;
let displayContext;

export function setupDisplay() {
    gameCanvas = document.getElementsByTagName('canvas')[0];
    
    if(gameCanvas) {
        gameContext = gameCanvas.getContext('2d');

        gameCanvas.style.display = 'none';

        displayCanvas = document.createElement('canvas');
        displayContext = displayCanvas.getContext('2d');

        
        let CanvasOverlay = document.getElementById("CanvasOverlay");
        CanvasOverlay.appendChild(displayCanvas);


        CanvasOverlay.style.width = '100vw';
        CanvasOverlay.style.height = '100vh';
        CanvasOverlay.style.position = 'absolute'; // Ensure it covers everything
        CanvasOverlay.style.top = '0';
        CanvasOverlay.style.left = '0';
        CanvasOverlay.style.overflow = 'hidden';

        displayCanvas.width = 1080;
        displayCanvas.height = 720;
        displayCanvas.style.width = '100vw';       // Fill parent's width
        displayCanvas.style.height = '100vh';      // Fill parent's height
        displayCanvas.style.position = 'absolute'; // Position absolutely within CanvasOverlay
        displayCanvas.style.top = '0';
        displayCanvas.style.left = '0';
        displayCanvas.style.zIndex = '9999';        

        displayContext.imageSmoothingEnabled = false;
        displayContext.mozImageSmoothingEnabled = false;   // For older Firefox versions
        displayContext.webkitImageSmoothingEnabled = false; // For older Safari and Chrome versions
        displayContext.msImageSmoothingEnabled = false;
    }
}

export function renderDisplayCanvas() {
    displayContext.drawImage(
        gameCanvas,
        0, 0,
        gameCanvas.width,
        gameCanvas.height,
        0, 0,
        displayCanvas.width,
        displayCanvas.height
    );
}


export function disableImageSmoothing() {
    const canvas = document.getElementsByTagName('canvas')[0];
    if (canvas) {
        const ctx = canvas.getContext('2d');
        console.log("Canvas found");

        try {
            ctx.imageSmoothingEnabled = false;
            ctx.mozImageSmoothingEnabled = false;   // For older Firefox versions
            ctx.webkitImageSmoothingEnabled = false; // For older Safari and Chrome versions
            ctx.msImageSmoothingEnabled = false;     // For older IE and Edge versions
        }
        catch {
            console.log("Something went wrong with smoothing");
        }
        
    } else {
        console.error('No canvas element found on the page.');
    }
}

export function getBrowserDimensions() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
}

let resizeEventHandler = null;
let dotNetReference = null;

export function setupResizeHandler(componentReference) {
    dotNetReference = componentReference;
    resizeEventHandler = () => {
        dotNetReference.invokeMethodAsync('OnBrowserResize');
    };
    window.addEventListener('resize', resizeEventHandler);
}

export function removeResizeHandler() {
    if (resizeEventHandler) {
        window.removeEventListener('resize', resizeEventHandler);
        resizeEventHandler = null;
        dotNetReference = null;
    }
}

//Audio handling

const loopingSounds = {};

export function playSound(path, volume, loop) {
    try {
        var audio = new Audio(path);
        audio.volume = volume;
        audio.loop = loop;
        audio.play();
        if (loop) {
            loopingSounds[path] = audio;
        }

    } catch (error) {
        console.error("Error playing sound:", error);
    }
}

export function stopSound(path) {
    if (loopingSounds[path]) {
        loopingSounds[path].pause();
        loopingSounds[path] = null;
    }
}

export function updateMusicVolume(volume) {
    for (const path in loopingSounds) {
        console.log("Accessing update music volume func");
        if (loopingSounds.hasOwnProperty(path) && loopingSounds[path]) {
            loopingSounds[path].volume = volume;
            console.log("Changing volume of looping sound");
        }
    }
}
