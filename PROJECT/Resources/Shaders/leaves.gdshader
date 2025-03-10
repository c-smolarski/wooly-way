shader_type canvas_item;

uniform vec3 lighterColor : source_color = vec3(0.486, 0.96, 0.501);
uniform vec3 darkerColor : source_color = vec3(0.0, 0.0, 0.0);

void fragment() {
	COLOR = mix(vec4(darkerColor, COLOR.a), vec4(lighterColor, COLOR.a), COLOR);
}