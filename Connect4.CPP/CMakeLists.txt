cmake_minimum_required(VERSION 3.0)
PROJECT (Connect4)

# Sources
SET(SOURCES
    main.cpp)

# Create the library
ADD_EXECUTABLE (${PROJECT_NAME} ${SOURCES})
INCLUDE_DIRECTORIES(Socket) 
TARGET_LINK_LIBRARIES(
    ${PROJECT_NAME}
)

SOURCE_GROUP ("App" FILES ${SOURCES})