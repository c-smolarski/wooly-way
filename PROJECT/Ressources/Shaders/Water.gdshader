shader_type canvas_item;

const vec4 FOAM_COLOR = vec4(1,1,1,1);
const mat2 isoMatrix = mat2(vec2(1, 0.5), vec2(-1, 0.5));

uniform float TILE_SIZE = 250.0;
uniform vec4 WATER_COLOR : source_color = vec4(0.094, 0.729, 0.911, 1.0);
uniform vec4 DARK_WATER_COLOR : source_color = vec4(0.066, 0.484, 0.689, 1.0);
uniform float FLOW_SPEED = 1;
uniform float COLOR_RANGE = 0.7;
uniform float COLOR_THRESHOLD = 0.65;
uniform float LIGHT_LINES_THICKNESS = 1.2;
uniform float FOAM_LINES_THICKNESS = 0.8;

varying vec2 position;

//returns pseudo-random value.
vec2 rand(vec2 uv) {
	return vec2(fract(sin(dot(uv.xy,
		vec2(12.9898,78.233))) * 43758.5453123));
}

//returns pixel color based on its position in order to make worley noise.
vec4 worley(vec2 pPixelPos, float pLineThickness){
	vec2 adjacentPos, currentPos = fract(pPixelPos);
	float dist, minDistBetweenPoints = 1000000.0;

	//checks for all adjacent tile on grid
	for(int y = 0; y < 3; y++)
		for(int x = 0; x < 3; x++){
			adjacentPos = vec2(float(x) - 1.0, float(y) - 1.0); //index - 1 because index are 0 based and we want -1 to +1
			adjacentPos += sin(TIME * rand(floor(pPixelPos) + adjacentPos)) * 0.25; //randomly moving tile centers
			dist = length(currentPos - adjacentPos);
			minDistBetweenPoints = min(dist, minDistBetweenPoints);
		}
	return vec4(smoothstep(1.0 - pLineThickness, 1.0, vec3(minDistBetweenPoints)), 1.0);
}

float blend(float pNoise){
	return smoothstep(COLOR_THRESHOLD - COLOR_RANGE, COLOR_THRESHOLD + COLOR_RANGE, pNoise);
}

vec4 getColor(vec2 pUV){
	vec4 water = mix(DARK_WATER_COLOR, WATER_COLOR, blend(worley(pUV, LIGHT_LINES_THICKNESS).r));
	return mix(water, FOAM_COLOR, blend(worley(-pUV * 2.0, FOAM_LINES_THICKNESS).r));
}

//sets position as the vertex's world position.
//Useful for scaling, expands the shader instead of stretching it.
void vertex(){
	position = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}

void fragment() {
	vec2 flowingMovement = vec2(isoMatrix * vec2(0,-1) * TIME * FLOW_SPEED); //adds isometric movment like flowing water.
	COLOR = getColor(position/TILE_SIZE + flowingMovement);
}