""" Example Connect4 Bot written in Python """
import requests  #python -m pip install requests

# The location of the game server
SERVER_URL = 'http://yorkdojoconnect4.azurewebsites.net/'

# The (unique) name of your team
TEAM_NAME = 'pythonExample'

# A password which must be supplied when you make a move
PASSWORD = 'MyPassword'

# Possible game states
GAMESTATE_GAMENOTSTARTED = 0
GAMESTATE_REDWON = 1
GAMESTATE_YELLOWWON = 2
GAMESTATE_REDTOPLAY = 3
GAMESTATE_YELLOWTOPLAY = 4
GAMESTATE_DRAW = 5

# Possible cell states
CELL_EMPTY = 0
CELL_RED = 1
CELL_YELLOW = 2

# Board size
NUMBER_OF_COLUMNS = 7
NUMBER_OF_ROWS = 6

# API Calls
def register_team():
    """ Call to register your team on the connect4 server"""
    url = SERVER_URL + 'api/Register?teamName=' + TEAM_NAME + '&password=' + PASSWORD
    response = requests.post(url)
    return response.text.strip('"')

def get_current_game(player_id):
    """ Call to get the current state of the game """
    url = SERVER_URL + 'api/GameState?playerID=' + player_id
    response = requests.get(url)
    return response.json()

def make_move(player_id, column_number):
    """ Call when you wish to place a counter in a column """
    url = SERVER_URL + 'api/MakeMove?playerID=' + player_id
    url = url + '&columnNumber='+str(column_number)+'&password=' + PASSWORD
    requests.post(url)
    return

# Helper methods
def cell(game, column_number, row_number):
    """ Returns the contents of a specified cell """
    return game['Cells'][column_number][row_number]

def is_column_full(game, column_number):
    """ Is there space in this column to place a counter?"""
    return cell(game, column_number, NUMBER_OF_ROWS - 1) != CELL_EMPTY

def column_height(game, column_number):
    """ How many counters are currently in this column """
    for row_number in range(0, NUMBER_OF_ROWS - 1):
        if cell(game, column_number, row_number) == CELL_EMPTY:
            return row_number
    return NUMBER_OF_ROWS  #Column is full

def is_winning_move(game, column_to_play, play_yellow):
    """ Would playing in this column result in a win """

    # Which row would the counter go in?
    row_to_play = column_height(game, column_to_play)

    # What colour counter are we looking for
    counter_to_match = CELL_YELLOW if play_yellow else CELL_RED

    # Do we complete 4 counters going downwards from here?
    row_to_check = row_to_play - 1
    column_to_check = column_to_play
    chain_length = 0
    while row_to_check >= 0 and (cell(game, column_to_check, row_to_check) == counter_to_match):
        row_to_check = row_to_check-1
        chain_length = chain_length+1
    if chain_length >= 3:
        return True

    # Left to right
    row_to_check = row_to_play
    column_to_check = column_to_play - 1
    chain_length = 0
    while column_to_check >= 0 and (cell(game, column_to_check, row_to_check) == counter_to_match):
        column_to_check = column_to_check - 1
        chain_length = chain_length+1
    if chain_length >= 3:
        return True

    # Right to Left
    column_to_check = column_to_play + 1
    chain_length = 0
    while column_to_check < NUMBER_OF_COLUMNS and (cell(game, column_to_check, row_to_check) == counter_to_match):
        column_to_check = column_to_check + 1
        chain_length = chain_length+1
    if chain_length >= 3:
        return True

    # +ve diagonal        

    # Not a winning move
    return False


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
