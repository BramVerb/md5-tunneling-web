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

    const shared = getSourceSynch("shared.fs");
    const vshader = getSourceSynch("vshader.vs");
    const fshader = shared+getSourceSynch("fshader.fs");
    const fshader2 = shared+getSourceSynch("fshaderblock2.fs");
    this.shaderProgram = initShaderProgram(this.gl, vshader, fshader);
    this.shaderProgram2 = initShaderProgram(this.gl, vshader, fshader2);
    this.programInfo = {
      program: this.shaderProgram,
      attribLocations: {
        vertexPosition: this.gl.getAttribLocation(
          this.shaderProgram,
          "vertexPosition"
        ),
      },
      uniformLocations: this.getUniforms(["seed"], this.gl, this.shaderProgram),
      // uniformLocations: {
      //   // projectionMatrix: gl.getUniformLocation(
      //   //   shaderProgram,
      //   //   "uProjectionMatrix"
      //   // ),
      //   seed: this.gl.getUniformLocation(this.shaderProgram, "seed"),
      //   // A0: this.gl.getUniformLocation(this.shaderProgram, "A0"),
      //   // B0: this.gl.getUniformLocation(this.shaderProgram, "B0"),
      //   // C0: this.gl.getUniformLocation(this.shaderProgram, "C0"),
      //   // D0: this.gl.getUniformLocation(this.shaderProgram, "D0"),
      //   // A1: this.gl.getUniformLocation(this.shaderProgram, "A1"),
      //   // B1: this.gl.getUniformLocation(this.shaderProgram, "B1"),
      //   // C1: this.gl.getUniformLocation(this.shaderProgram, "C1"),
      //   // D1: this.gl.getUniformLocation(this.shaderProgram, "D1"),
      // },
    };


    this.programInfo2 = {
      program: this.shaderProgram2,
      attribLocations: {
        vertexPosition: this.gl.getAttribLocation(
          this.shaderProgram2,
          "vertexPosition"
        ),
      },
      uniformLocations: this.getUniforms(["seed"], this.gl, this.shaderProgram2),
      // {
      //   // projectionMatrix: gl.getUniformLocation(
      //   //   shaderProgram,
      //   //   "uProjectionMatrix"
      //   // ),
      //   seed: this.gl.getUniformLocation(this.shaderProgram2, "seed"),
      //   // A0: this.gl.getUniformLocation(this.shaderProgram, "A0"),
      //   // B0: this.gl.getUniformLocation(this.shaderProgram, "B0"),
      //   // C0: this.gl.getUniformLocation(this.shaderProgram, "C0"),
      //   // D0: this.gl.getUniformLocation(this.shaderProgram, "D0"),
      //   // A1: this.gl.getUniformLocation(this.shaderProgram, "A1"),
      //   // B1: this.gl.getUniformLocation(this.shaderProgram, "B1"),
      //   // C1: this.gl.getUniformLocation(this.shaderProgram, "C1"),
      //   // D1: this.gl.getUniformLocation(this.shaderProgram, "D1"),
      // },
    };
    this.quad = initBuffers(this.gl);
    this.generator = new Block1CandidatesGenerator(4);
    this.collisions = [];
    this.pixelValues = new Uint8Array(256 * 256 * 4);
    this.start = Date.now();
    this.last = Date.now()+1;
    this.frames = 0;
    this.next = this.generator.getnext(100000);
  }

  getUniforms(uniformNames, gl, shaderProgram) {
    const uniforms = {};
    uniformNames.forEach((name) => {
      uniforms[name] = gl.getUniformLocation(shaderProgram, name);
    });
    return uniforms;
  }

  setupScene() {
    this.gl.useProgram(this.programInfo.program);

    if(this.collisions.length == 100) {
      // const c = this.collisions.pop();
      // const {x, y, v, seed} = c;
      // this.determineTunnelValues(x, y, v, seed);
      // this.gl.uniform1ui(this.programInfo.uniformLocations.seed, seed >>> 0);
      // this.gl.uniform1ui(this.programInfo.uniformLocations.A0, A0 >>> 0);
      // this.gl.uniform1ui(this.programInfo.uniformLocations.B0, B0 >>> 0);
      // this.gl.uniform1ui(this.programInfo.uniformLocations.C0, C0 >>> 0);
      // this.gl.uniform1ui(this.programInfo.uniformLocations.D0, D0 >>> 0);
      // this.gl.uniform1ui(this.programInfo.uniformLocations.A1, A1 >>> 0);
      // this.gl.uniform1ui(this.programInfo.uniformLocations.B1, B1 >>> 0);
      // this.gl.uniform1ui(this.programInfo.uniformLocations.C1, C1 >>> 0);
      // this.gl.uniform1ui(this.programInfo.uniformLocations.D1, D1 >>> 0);
      this.block = 2;
      return;
    } else {
      this.block = 1;
    }
    const candidate = this.next;
    if (candidate) {
      const seed = candidate.seed;
      this.gl.uniform1ui(this.programInfo.uniformLocations.seed, seed >>> 0);
    } else {
      console.warn("found no candidate");
    }
    this.gl.uniform1ui(this.programInfo.uniformLocations.block, this.block >>> 0);
  }

  determineTunnelValues(x, y, output, seed) {
    let id = (x + y * 256) >>> 0;

    const Q4_strength = 1;
    const startQ4 = id & ((1 << Q4_strength) - 1);
    id = id >>> Q4_strength;


    const Q9_strength = 3;
    const startQ9 = id & ((1 << Q9_strength) - 1);
    id = id >>> Q9_strength;

    const Q13_strength = 12;
    const startQ13 = id & ((1 << Q13_strength) - 1);
    id = id >>> Q13_strength;

    const Q14_strength = 9;
    const startQ14 = output & ((1 << Q14_strength)-1);
    output = output >>> Q14_strength;
    const Q10_strength = 3;
    const startQ10 = output & ((1 << Q10_strength)-1);
    output = output >>> Q10_strength;
    const Q20_strength = 6;
    const startQ20 = output & ((1 << Q20_strength)-1);
    output = output >>> Q20_strength;
    const input = {
      id,
      seed,
      startQ4,
      startQ9,
      startQ13,
      startQ10,
      startQ20,
      startQ14,
    };
    console.log('collision', Block1(input));
    // const a = String.fromCharCode(...v1.slice(0, 64))
    // const b = String.fromCharCode(...v2.slice(0, 64))
    // newBlock1(a, b);
    console.time('block 2');
    // console.log('collision block 2', Block2(input));
    console.timeEnd('block 2');
  }

  readFrame() {
    this.frames += 1;
    const width = 256;
    const height = 256;
    const oldLast = this.last;
    const seed = this.next.seed;
    for (let x = 0; x < width; x++) {
      for (let y = 0; y < height; y++) {
        let v = 0;
        for (let i = 0; i < 3; i++) {
          const index = x * 4 + y * width * 4 + i;
          const value = this.pixelValues[index];
          if (value > 0) {
            if (i !== 3) {
              v += value << (8 * i);
              console.log(v, value);
            }
          }
        }
        if (v > 0) {
          if(this.block === 1) {
            console.log(`new collision: at x=${x}, y=${y}: v: ${v >>> 0}`);
            this.collisions.push({ x, y, v, seed });
            this.last = Date.now();
            this.determineTunnelValues(x, y, v, seed);
          } else if(this.block === 2){
            console.log(`new second block collision: at x=${x}, y=${y}: v: ${v >>> 0}`);
          }
        }
      }
    }
    if(this.last != oldLast) {
      const seconds = (this.last - this.start) / 1000;
      console.log("collisions:", this.collisions.length, "in", Math.floor(seconds) , 'persecond: ', this.collisions.length / seconds, "fps:", this.frames / seconds);
    }
  }

  frame() {
    let error = this.gl.getError();
    if(error){
      console.log('gl error', error);
      return;
    }
    console.log('this.frame');
    if(this.next) {
      this.setupScene();
      drawScene(this.gl, this.programInfo, this.quad);
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
    }
    this.next = this.generator.getnext(100000)
    requestAnimationFrame(this.frame.bind(this));
  }
}

function gpu() {
    console.log('renderer');
    const renderer = new Renderer();
    renderer.frame();
}

function bottom() {
    document.getElementById( 'bottom' ).scrollIntoView({behavior: 'smooth'});
    window.setTimeout( function () { bottom(); }, 1000 );
};
// bottom()
setTimeout(gpu, 100);
