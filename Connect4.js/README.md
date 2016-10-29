# connect4-js

This project provides a skeleton structure for creating a connect 4 bot.

## Getting Started

### Command line

```shell
npm install && npm build && node .
```

### Prerequisites

1. Clone, fork, or download the project.
1. You need Node.js. [Go install it][nodejsdownload].
1. Ensure the required dependencies have been installed:
    ```bash
    npm install
    ```

1. You will need [`typings`][typings] to allow the TypeScript to compile without errors. It's recommended to install this globally:
    ```bash
    npm install typings -g
    ```

1. Change to the `src` directory and run `typings install` to fetch the required module type definitions defined in `typings.json`:
    ```bash
    cd src

    # if installed globally (recommended)
    typings install

    # otherwise
    ../node_modules/.bin/typings install
    ```

### Building with VSCode

1. Open VSCode, hit <kbd>CTRL</kbd>/<kbd>Cmd</kbd>+<kbd>Shift</kbd>+<kbd>P</kbd>, type `open folder` and select the root of this repository
1. Build with one of the following shortcuts:
   * Press <kbd>CTRL</kbd>/<kbd>Cmd</kbd>+<kbd>Shift</kbd>+<kbd>B</kbd> to build, which is declared in the `.vscode/tasks.json` file with the `isBuildCommand` marker
   * Press <kbd>CTRL</kbd>/<kbd>Cmd</kbd>+<kbd>Shift</kbd>+<kbd>P</kbd> and select the `Tasks: Run Build Task` option
   * Press <kbd>CTRL</kbd>/<kbd>Cmd</kbd>+<kbd>P</kbd> and type `task build`

### Error Navigation

After building or testing, errors are captured (defined in the `.vscode/tasks.json` file) and can be viewed with <kbd>CTRL</kbd>/<kbd>Command</kbd>+<kbd>Shift</kbd>+<kbd>M</kbd>.

Your `.ts` files have been compiled to `.js` files within the `build` directory, and each should have a `.js.map` _sourcemap_ file alongside it to allow stack traces to correctly report the line in the original file. See [this StackOverflow article][sourcemapquestion] for an overview of what a sourcemap is.

### Testing

There are sample tests located in the `test` folder. You can run them by hitting <kbd>CTRL</kbd>/<kbd>Command</kbd>+<kbd>Shift</kbd>+<kbd>T</kbd> (or use the `Tasks` menu and run `Tasks: Run Test Task`)

### Running and Debugging in VSCode

To run the project in debug mode, simply hit <kbd>F5</kbd>! Place breakpoints in your TypeScript code and view them in the debugger (<kbd>CTRL</kbd>+<kbd>Shift</kbd>+<kbd>D</kbd> or <kbd>Cmd</kbd>+<kbd>Shift</kbd>+<kbd>D</kbd>).

## Project Structure

This project is based on (vscode-typescript-boilerplate)[vscode-typescript-boilerplate]

## License

MIT

[vscode]: https://code.visualstudio.com/
[nodejsdownload]: https://nodejs.org/download/
[sourcemapquestion]: http://stackoverflow.com/questions/21719562/javascript-map-files-javascript-source-maps
[typings]: https://www.npmjs.com/package/typings
[vscode-typescript-boilerplate]: https://github.com/Codesleuth/vscode-typescript-boilerplate

