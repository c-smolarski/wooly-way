shader_type particles;

uniform float rows : hint_range(1, 120) = 16.0;
uniform float spacing : hint_range(0.1, 10.0) = 1.0;

void start() {
    // Calcule la position dans la grille
    vec3 pos = vec3(0.0);

    // Détermine la ligne et la colonne de la particule
    pos.y = float(INDEX);
    pos.x = mod(pos.y, rows);
    pos.y = (pos.y - pos.x) / rows;

    // Centre la grille
    pos.x -= rows * 0.5;
    pos.y -= rows * 0.5;

    // Applique l'espacement entre les particules
    pos *= spacing;

    // Récupère la position d'émission depuis la matrice de transformation
    vec2 emission_pos = vec2(EMISSION_TRANSFORM[3].x, EMISSION_TRANSFORM[3].y);

    // Centre la position autour du point d'émission, en alignant sur la grille
    pos.x += emission_pos.x - mod(emission_pos.x, spacing);
    pos.y += emission_pos.y - mod(emission_pos.y, spacing);

    // Applique la nouvelle position à la transformation de la particule
    TRANSFORM[3].x = pos.x;
    TRANSFORM[3].y = pos.y;

    // Stocke la position dans CUSTOM pour potentiels effets futurs
    CUSTOM.xy = pos.xy;
}
