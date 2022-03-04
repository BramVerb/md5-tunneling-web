//
// creates a shader of the given type, uploads the source and
// compiles it.
//
function loadShader(gl, type, source, name) {
  const shader = gl.createShader(type);

  // Send the source to the shader object

  gl.shaderSource(shader, source);

  // Compile the shader program

  gl.compileShader(shader);

  // See if it compiled successfully

  if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
    console.error(
      `An error occurred compiling the shaders: ${name}\n` +
        gl.getShaderInfoLog(shader)
    );
    gl.deleteShader(shader);
    return null;
  }

  return shader;
}

function initShaderProgram(gl, vsSource, fsSource) {
  const vertexShader = loadShader(gl, gl.VERTEX_SHADER, vsSource, "VERTEX");
  const fragmentShader = loadShader(
    gl,
    gl.FRAGMENT_SHADER,
    fsSource,
    "FRAGMENT"
  );

  // Create the shader program

  const shaderProgram = gl.createProgram();
  gl.attachShader(shaderProgram, vertexShader);
  gl.attachShader(shaderProgram, fragmentShader);
  gl.linkProgram(shaderProgram);

  // If creating the shader program failed, alert

  if (!gl.getProgramParameter(shaderProgram, gl.LINK_STATUS)) {
    console.error(
      "Unable to initialize the shader program: " +
        gl.getProgramInfoLog(shaderProgram)
    );
    return null;
  }

  return shaderProgram;
}

var getSourceSynch = function (url) {
  var req = new XMLHttpRequest();
  req.open("GET", url, false);
  req.send(null);
  return req.status == 200 ? req.responseText : null;
};

function initBuffers(gl) {
  // Create a buffer for the square's positions.

  const positionBuffer = gl.createBuffer();

  // Select the positionBuffer as the one to apply buffer
  // operations to from here out.

  gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);

  // Now create an array of positions for the square.

  const positions = [1.0, 1.0, -1.0, 1.0, 1.0, -1.0, -1.0, -1.0];

  // Now pass the list of positions into WebGL to build the
  // shape. We do this by creating a Float32Array from the
  // JavaScript array, then use it to fill the current buffer.

  gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(positions), gl.STATIC_DRAW);

  return {
    position: positionBuffer,
  };
}

function drawScene(gl, programInfo, buffers) {
  gl.clearColor(0.0, 0.0, 0.0, 1.0); // Clear to black, fully opaque
  gl.clearDepth(1.0); // Clear everything
  gl.enable(gl.DEPTH_TEST); // Enable depth testing
  gl.depthFunc(gl.LEQUAL); // Near things obscure far things

  // Clear the canvas before we start drawing on it.

  gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);

  // Tell WebGL how to pull out the positions from the position
  // buffer into the vertexPosition attribute.
  {
    const numComponents = 2; // pull out 2 values per iteration
    const type = gl.FLOAT; // the data in the buffer is 32bit floats
    const normalize = false; // don't normalize
    const stride = 0; // how many bytes to get from one set of values to the next
    // 0 = use type and numComponents above
    const offset = 0; // how many bytes inside the buffer to start from
    gl.bindBuffer(gl.ARRAY_BUFFER, buffers.position);
    gl.vertexAttribPointer(
      programInfo.attribLocations.vertexPosition,
      numComponents,
      type,
      normalize,
      stride,
      offset
    );
    gl.enableVertexAttribArray(programInfo.attribLocations.vertexPosition);
  }

  // Tell WebGL to use our program when drawing

  gl.useProgram(programInfo.program);

  // Set the shader uniforms

  {
    const offset = 0;
    const vertexCount = 4;
    gl.drawArrays(gl.TRIANGLE_STRIP, offset, vertexCount);
  }
}

function frame() {}

class Renderer {
  constructor() {
    const canvas = document.getElementById("canvas");
    this.gl = canvas.getContext("webgl2");
    this.gl.imageSmoothingEnabled = false;
    if (this.gl == null) {
      alert("unable to init opengl");
      return;
    }
    // Set clear color to black, fully opaque
    this.gl.clearColor(0.0, 0.0, 0.0, 1.0);
    // Clear the color buffer with specified clear color
    this.gl.clear(this.gl.COLOR_BUFFER_BIT);
    console.log(this.gl.getParameter(this.gl.SHADING_LANGUAGE_VERSION));

    const vshader = getSourceSynch("vshader.vs");
    const fshader = getSourceSynch("fshader.fs");
    this.shaderProgram = initShaderProgram(this.gl, vshader, fshader);
    this.programInfo = {
      program: this.shaderProgram,
      attribLocations: {
        vertexPosition: this.gl.getAttribLocation(
          this.shaderProgram,
          "vertexPosition"
        ),
      },
      uniformLocations: {
        // projectionMatrix: gl.getUniformLocation(
        //   shaderProgram,
        //   "uProjectionMatrix"
        // ),
        seed: this.gl.getUniformLocation(this.shaderProgram, "seed"),
      },
    };
    this.quad = initBuffers(this.gl);
    this.generator = new Block1CandidatesGenerator(401223190239012);
    this.collisions = [];
  }

  setupScene() {
    // const seed = 2382399738;
    // const seed = 4;
    // const seed = 3770369038;
    const candidate = this.generator.getnext(10000000);
    if (candidate) {
      const seed = candidate.seed;
      // const seed = 489166028;
      this.gl.useProgram(this.programInfo.program);
      this.gl.uniform1ui(this.programInfo.uniformLocations.seed, seed >>> 0);
    } else {
      console.log("found no candidate");
      return false;
    }
    // gl.uniformMatrix4fv(
    //     programInfo.uniformLocations.projectionMatrix,
    //     false,
    //     projectionMatrix);

    // A0 13732443
    // A0 1481994324
  }

  readFrame() {
    // console.log('reading frame');
    const width = 256;
    const height = 256;
    for (let x = 0; x < width; x++) {
      for (let y = 0; y < height; y++) {
        let v = 0;
        let found = false;
        for (let i = 0; i < 4; i++) {
          const index = x * 4 + y * width * 4 + i;
          const value = this.pixelValues[index];
          if (value > 0) {
            if (i !== 3) {
              v += value << (8 * i);
            }
            found = true;
          }
        }
        if (v > 0) {
          console.log(`v at x=${x}, y=${y}: ${v >>> 0}`);
          this.collisions.push({ x, y, v });
        }
      }
    }
    console.log("collisions:", this.collisions.length);
    // console.log('done reading frame');
  }

  frame() {
    this.setupScene();
    drawScene(this.gl, this.programInfo, this.quad);
    this.pixelValues = new Uint8Array(256 * 256 * 4);
    this.gl.readPixels(
      0,
      0,
      256,
      256,
      this.gl.RGBA,
      this.gl.UNSIGNED_BYTE,
      this.pixelValues
    );
    this.readFrame();
    // requestAnimationFrame(this.readFrame.bind(this));
    requestAnimationFrame(this.frame.bind(this));
    this.next = new Promise((resolve, reject) => {});
  }
}

function gpu() {
  const renderer = new Renderer();
  renderer.frame();
}
// setTimeout(gpu, 100);
