shader_type canvas_item;

uniform sampler2D spritesheet;
uniform sampler2D random_noise;
uniform sampler2D wind_noise;

uniform int frames : hint_range(1, 64) = 4;
uniform vec4 top_color : source_color = vec4(0.2, 0.8, 0.0, 1.0);
uniform vec4 bottom_color : source_color = vec4(0.15, 0.6, 0.0, 1.0);
uniform float wind_scale = 1.0;
uniform float wind_speed = 0.1;
uniform float wind_strength = 0.1;

uniform vec2 player_pos = vec2(0.0);
uniform float player_radius = 20.0;
uniform float player_radius_blend = 10.0;

varying vec2 world_pos;
varying float player_mask;

// Utility function for rotating UVs
vec2 rotateUV(vec2 uv, float rotation, vec2 pivot) {
    float cos_a = cos(rotation);
    float sin_a = sin(rotation);
    vec2 offset = uv - pivot;
    return vec2(
        cos_a * offset.x - sin_a * offset.y,
        sin_a * offset.x + cos_a * offset.y
    ) + pivot;
}

void vertex() {
    world_pos = (INSTANCE_CUSTOM.xy);
    
    // Calculate wind effect
    float wind = texture(wind_noise, (world_pos / wind_scale) + TIME * wind_speed).r;
    wind = wind * 2.0 - 1.0;

    // Player interaction mask
    float player_distance = distance(world_pos, player_pos);
    player_mask = smoothstep(player_radius, player_radius + player_radius_blend, player_distance);

    // Calculate angle from player to grass blade
    vec2 player_dir = normalize(world_pos - player_pos);
    float signed_angle = atan(player_dir.y, player_dir.x);

    // Combine wind and player interaction for rotation
    float final_rotation = mix(signed_angle, wind * wind_strength, player_mask);
    VERTEX = rotateUV(VERTEX, final_rotation, vec2(0.0));
}

void fragment() {
    // Random number per grass blade based on world position
    vec2 rand_uv = world_pos;
    vec2 noise_tex_size = vec2(textureSize(random_noise, 0));
    rand_uv = mod(rand_uv, noise_tex_size);
    float rand_value = texelFetch(random_noise, ivec2(rand_uv), 0).r;

    // Determine the frame index from random value
    float frame_number = round(rand_value * float(frames));
    float frame_width = 1.0 / float(max(frames, 1));
    float x_coord = mix(frame_width * frame_number, frame_width * (frame_number + 1.0), UV.x);

    vec2 midpoint = vec2(frame_width * frame_number + frame_width * 0.5, 0.5);
    vec2 uv = vec2(x_coord, UV.y);

    // Sample the sprite and apply color gradient
    vec4 sprite_color = texture(spritesheet, uv);
    vec3 blended_color = mix(top_color.rgb, bottom_color.rgb, UV.y);
    blended_color *= sprite_color.rgb;

    // Final output with transparency affected by player mask
    COLOR = vec4(blended_color, sprite_color.a * player_mask);
}
