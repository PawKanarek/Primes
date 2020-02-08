#version 330
out vec4 outputColor;

in vec2 texCoord;
uniform sampler2D texture0;

void main()
{
    outputColor = texture(texture0, texCoord);
    //outputColor = vec4(1.0, 1.0, 0.3, 1.0);
}