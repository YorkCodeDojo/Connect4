""" Unit tests - run pytest """
from Connect4Helpers import (is_column_full, column_height, is_winning_move,
                             NUMBER_OF_COLUMNS)

def define_board(rows_of_columns):
    """ Transposes the board """
    columns_of_rows = list(map(list, zip(*rows_of_columns)))
    for column_number in range(0, NUMBER_OF_COLUMNS):
        columns_of_rows[column_number].reverse()
    return {"Cells":columns_of_rows}

def test_that_an_empty_column_is_not_full():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0]])
    assert not is_column_full(board, 0)

def test_that_a_full_column_is_full():
    board = define_board([[0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0]])
    assert is_column_full(board, 2)

def test_that_a_partfull_column_is_not_full():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0]])
    assert not is_column_full(board, 2)


def test_that_an_empty_column_is_has_height_0():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0]])
    assert column_height(board, 0) == 0

def test_that_an_empty_column_is_has_height_6():
    board = define_board([[0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0]])
    assert column_height(board, 2) == 6

def test_that_a_part_column_has_height_5():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0],
                          [0, 0, 1, 0, 0, 0, 0]])
    assert column_height(board, 2) == 5

def test_that_a_nearly_empty_column_has_height_1():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 2]])
    assert column_height(board, 6) == 1

 #0=CLEAR, 1=RED,  2=YELLOW
def test_playing_into_an_empty_board_does_not_win():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0]])
    assert not is_winning_move(board, 6, True) 

def test_left_win_on_bottom_line():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [1, 1, 1, 0, 0, 0, 0]])
    assert is_winning_move(board, 3, False)

def test_right_win_on_bottom_line():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 1, 1, 0, 1, 1, 1]])
    assert is_winning_move(board, 3, False)    

def test_nowin_on_bottom_line():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 1, 0, 0, 0, 0, 0],
                          [0, 1, 0, 0, 0, 0, 0],
                          [1, 1, 0, 0, 0, 0, 0]])
    assert not is_winning_move(board, 2, False)


def test_mid_line_win():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 1, 0, 0, 0, 0, 0],
                          [2, 2, 0, 2, 0, 0, 0],
                          [1, 1, 1, 2, 0, 0, 0]])
    assert not is_winning_move(board, 2, True)

def test_vertical_win():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 2, 0, 0, 0],
                          [0, 0, 0, 2, 0, 0, 0],
                          [0, 0, 0, 2, 0, 0, 0],
                          [0, 0, 0, 1, 0, 0, 0],
                          [1, 1, 1, 1, 0, 0, 0]])
    assert is_winning_move(board, 3, True)
    
def test_novertical_win():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 2, 0, 0, 0],
                          [0, 0, 0, 2, 0, 0, 0],
                          [0, 0, 0, 2, 0, 0, 0],
                          [0, 0, 0, 1, 0, 0, 0],
                          [1, 1, 1, 1, 0, 0, 0]])
    assert not is_winning_move(board, 3, False)    

def test_neg_pos_diag_win():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 2, 2, 0, 0, 0],
                          [0, 2, 1, 1, 0, 0, 0],
                          [2, 1, 1, 1, 0, 0, 0]])
    assert is_winning_move(board, 3, True)    

def test_pos_pos_diag_win():
    board = define_board([[0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 0, 0, 0, 0],
                          [0, 0, 0, 2, 0, 0, 0],
                          [0, 0, 2, 2, 0, 0, 0],
                          [0, 2, 1, 1, 0, 0, 0],
                          [0, 1, 1, 1, 0, 0, 0]])
    assert is_winning_move(board, 0, True)

def test_pos_pos_diag_mid_win():
    board = define_board([[0, 0, 0, 0, 0, 2, 0],
                          [0, 0, 0, 0, 0, 1, 0],
                          [0, 0, 0, 2, 1, 1, 0],
                          [0, 0, 2, 2, 1, 1, 0],
                          [0, 0, 1, 1, 1, 1, 0],
                          [0, 1, 1, 1, 1, 1, 0]])
    assert is_winning_move(board, 4, True)

def test_pos_pos_diag_mid_nowin():
    board = define_board([[0, 0, 0, 0, 0, 2, 0],
                          [0, 0, 0, 0, 0, 1, 0],
                          [0, 0, 0, 2, 1, 1, 0],
                          [0, 0, 0, 2, 1, 1, 0],
                          [0, 0, 1, 1, 1, 1, 0],
                          [0, 1, 1, 1, 1, 1, 0]])
    assert not is_winning_move(board, 4, True)

test_pos_pos_diag_win()    

