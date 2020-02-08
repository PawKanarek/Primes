#version 330 core

layout(location = 0) in vec3 vertexCoord;
layout(location = 1) in vec2 textureCoord;
 
out vec2 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    gl_Position = vec4(vertexCoord, 1.0) * model * view * projection;
    texCoord = textureCoord;
}