#define is_down(b) input->buttons[b].is_down
#define pressed(b) (input->buttons[b].is_down && input->buttons[b].changed)
#define released(b) (!input->buttons[b].is_down && input->buttons[b].changed)

float player1_p, player1_dp, player2_p, player2_dp;
float arena_half_size_x = 85, arena_half_size_y = 45;
float player_half_size_x = 2.5, player_half_size_y = 12;
float ball_p_x, ball_p_y, ball_dp_x = 100, ball_dp_y, ball_half_size = 1;

internal void simulate_game(Input* input, float dt)
{
	clear_screen(0x4f5763);

	draw_rect(0, 0, arena_half_size_x, arena_half_size_y, 0xffffff);

	float player1_ddp = 0.f;

	if (is_down(BUTTON_UP))
	{
		player1_ddp += 2000;

	}
	if (is_down(BUTTON_DOWN))
	{
		player1_ddp -= 2000;

	}
	float player2_ddp = 0.f;

	if (is_down(BUTTON_W))
	{
		player2_ddp += 2000;

	}
	if (is_down(BUTTON_S))
	{
		player2_ddp -= 2000;

	}

	// Check collisions with top border and player1
	if (player1_p + player_half_size_y > arena_half_size_y)
	{
		player1_p = arena_half_size_y - player_half_size_y;
		player1_dp *= 0;
	}

	// Check collisions with bottom border and player1
	else if (player1_p - player_half_size_y < -arena_half_size_y)
	{
		player1_p = -arena_half_size_y + player_half_size_y;
		player1_dp *= 0;
	}
	
	// Check collisions with top border and player2
	if (player2_p + player_half_size_y > arena_half_size_y)
	{
		player2_p = arena_half_size_y - player_half_size_y;
		player2_dp *= 0;
	}

	// Check collisions with bottom border and player2
	else if (player2_p - player_half_size_y < -arena_half_size_y)
	{
		player2_p = -arena_half_size_y + player_half_size_y;
		player2_dp *= 0;
	}

	// Player1 movement
	player1_ddp -= player1_dp * 10.f;

	player1_p = player1_p + player1_dp * dt + player1_ddp * dt * dt * .5f;
	player1_dp = player1_dp + player1_ddp * dt;
	
	// Player2 movement
	player2_ddp -= player2_dp * 10.f;

	player2_p = player2_p + player2_dp * dt + player2_ddp * dt * dt * .5f;
	player2_dp = player2_dp + player2_ddp * dt;

	ball_p_x += ball_dp_x * dt;
	ball_p_y += ball_dp_y * dt;

	//Draw ball
	draw_rect(ball_p_x, ball_p_y,1,1,0x000000);

	// Check collisions between ball and player1
	if (ball_p_x + ball_half_size > 80 - player_half_size_x &&
		ball_p_x - ball_half_size < 80 + player_half_size_x &&
		ball_p_y + ball_half_size > player1_p - player_half_size_y &&
		ball_p_y + ball_half_size < player1_p + player_half_size_y)
	{
		ball_p_x = 80 - player_half_size_x - ball_half_size;
		ball_dp_x *= -1;
		ball_dp_y = (ball_p_y - player1_p) * 2 + player1_dp * .75f;
	}
		
	// Check collisions between ball and player2
	else if (ball_p_x + ball_half_size > -80 - player_half_size_x &&
		ball_p_x - ball_half_size < -80 + player_half_size_x &&
		ball_p_y + ball_half_size > player2_p - player_half_size_y &&
		ball_p_y + ball_half_size < player2_p + player_half_size_y)
	{
		ball_p_x = -80 + player_half_size_x + ball_half_size;
		ball_dp_x *= -1;
		ball_dp_y = (ball_p_y - player2_p) * 2 + player2_dp * .75f;		
	}

	//Check collisions between ball and borders
	if (ball_p_y + ball_half_size > arena_half_size_y)
	{
		ball_p_y = arena_half_size_y - ball_half_size;
		ball_dp_y *= -1;
	}
	else if (ball_p_y - ball_half_size < -arena_half_size_y)
	{
		ball_p_y = -arena_half_size_y + ball_half_size;
		ball_dp_y *= -1;
	}

	//If collision with borders on x-axis, reset ball position to center
	if (ball_p_x + ball_half_size > arena_half_size_x)
	{
		ball_dp_x *= -1;
		ball_dp_y = 0;
		ball_p_x = 0;
		ball_p_y = 0;
	}

	else if (ball_p_x - ball_half_size < -arena_half_size_x)
	{
		ball_dp_x *= -1;
		ball_dp_y = 0;
		ball_p_x = 0;
		ball_p_y = 0;
	}

	//Draw player1
	draw_rect(80,player1_p, player_half_size_x, player_half_size_y,0xff0000);

	//Draw player2
	draw_rect(-80,player2_p,player_half_size_x, player_half_size_y,0x1367ed);
}