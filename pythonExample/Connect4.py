""" Example Connect4 Bot written in Python """
from Connect4Helpers import (is_column_full, make_move, register_team, get_current_game,
                             NUMBER_OF_COLUMNS, GAMESTATE_REDWON, GAMESTATE_YELLOWWON,
                             GAMESTATE_DRAW, GAMESTATE_YELLOWTOPLAY, GAMESTATE_REDTOPLAY)

# Put your game logic here
def workout_move(player_id, game, we_are_yellow):
    """ Works out and plays the next move"""

    # Play in the first column which isn't full.
    for column_number in range(0, NUMBER_OF_COLUMNS - 1):
        if not is_column_full(game, column_number):
            print("Playing into column " + str(column_number))
            make_move(player_id, column_number)
            return

def play_game():
    """ Main game loop logic """

    print("Connect 4 Example Bot")

    #Register your team,  this responds with your PlayerID
    player_id = register_team()

    # Main Game Loop,  we keep polling the server waiting for it to become our turn
    game_is_complete = False
    while not game_is_complete:

        # Get the current state of the game
        game = get_current_game(player_id)
        print(game)

        # What colour are we playing
        we_are_yellow = (game['YellowPlayerID'] == player_id)

        # What is the current state of the game
        current_state = game['CurrentState']

        if current_state == GAMESTATE_REDWON:
            # The red player has won
            print("Red has won")
            game_is_complete = True

        elif current_state == GAMESTATE_YELLOWWON:
            # The yellow player has won
            print("Yellow has won")
            game_is_complete = True

        elif current_state == GAMESTATE_DRAW:
            # It's a draw'
            print("It's a draw")
            game_is_complete = True

        elif current_state == GAMESTATE_YELLOWTOPLAY and we_are_yellow:
            # We are yellow and it's our turn to play
            workout_move(player_id, game, we_are_yellow)

        elif current_state == GAMESTATE_REDTOPLAY and not we_are_yellow:
            # We are red and it's our turn to play
            workout_move(player_id, game, we_are_yellow)

play_game()
