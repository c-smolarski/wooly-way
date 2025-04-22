shader_type particles;

uniform float rows = 16;
uniform float spacing = 1.0;
uniform vec2 polygon[32]; // array of vector2 of the exported polygon's vertices
uniform int polygon_size; //polygon.length

bool isInsidePolygon(vec2 point)
{
    bool inside = false;
    for (int i = 0, j = polygon_size - 1; i < polygon_size; j = i++)
    {
        vec2 vi = polygon[i];
        vec2 vj = polygon[j];

        if (((vi.y > point.y) != (vj.y > point.y)) &&
            (point.x < (vj.x - vi.x) * (point.y - vi.y) / (vj.y - vi.y) + vi.x))
        {
            inside = !inside;
        }
    }
    return inside;
}

void start() {
    // Base grid setup
    vec3 pos = vec3(0.0, 0.0, 0.0);
    pos.y = float(INDEX);
    pos.x = mod(pos.y, rows);
    pos.y = (pos.y - pos.x) / rows;

    // Apply spacing
    pos *= spacing;

    // Move the particles based on the upper-left corner (without centering)
    // align them to the top-left corner
    pos.x += EMISSION_TRANSFORM[3][0];
    pos.y += EMISSION_TRANSFORM[3][1];

	//TRANSFORM[3][0] = pos.x;
    //TRANSFORM[3][1] = pos.y;
    if (isInsidePolygon(pos.xy) || polygon_size < 4)
	{
    TRANSFORM[3][0] = pos.x;
    TRANSFORM[3][1] = pos.y;
	}
	else
	{
    // Move off-screen
    TRANSFORM[3][0] = -10000.0;
    TRANSFORM[3][1] = -10000.0;
	}

    // Pass the position to instances for custom use
    CUSTOM.xy = pos.xy;
}
