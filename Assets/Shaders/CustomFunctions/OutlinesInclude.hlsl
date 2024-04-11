#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED

static float2 sobelSamplePoints[9] = {
    float2(-1, 1), float2(0, 1), float2(1, 1),
    float2(-1, 0), float2(0, 0), float2(1, 0),
    float2(-1, -1), float2(0, -1), float2(1, -1)
};

static float sobelXMatrix[9] = {
    1, 0, -1,
    2, 0, -2,
    1, 0, -1
};

static float sobelYMatrix[9] = {
    1, 2, 1,
    0, 0, 0,
    -1, -2, -1
};

void DepthSobel_float(float2 UV, float PixelSize, out float Out)
{
    float2 sobel = 0;

    [unroll] for (int i = 0; i < 9; ++i)
    {
        float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV + sobelSamplePoints[i] * PixelSize);
        sobel += float2(sobelXMatrix[i], sobelYMatrix[i]) * depth;
    }

    Out = length(sobel);
}

void ColorSobel_float(float2 UV, float PixelSize, out float Out)
{
    float2 sobelR = 0;
    float2 sobelG = 0;
    float2 sobelB = 0;

    [unroll] for (int i = 0; i < 9; ++i)
    {
        float3 color = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV + sobelSamplePoints[i] * PixelSize);
        float2 kernel = float2(sobelXMatrix[i], sobelYMatrix[i]);
        sobelR += kernel * color.r;
        sobelG += kernel * color.g;
        sobelB += kernel * color.b;
    }
    Out = max(length(sobelR), max(length(sobelG), length(sobelB)));
}
#endif
