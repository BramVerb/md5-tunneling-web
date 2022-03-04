#version 300 es
precision highp float;
in vec2 vertexPosition;

out vec2 position;

void main(){
    gl_Position = vec4(vertexPosition.xy, 0.0, 1.0);
    position = (vertexPosition + 1.0) / 2.0;
}
