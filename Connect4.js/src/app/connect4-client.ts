import * as http from 'http';
import * as querystring from 'querystring';

export enum GameState {
  GameNotStarted = 0,
  RedWon = 1,
  YellowWon = 2,
  RedToPlay = 3,
  YellowToPlay = 4,
  Draw = 5
}

export enum CellContent {
  Empty = 0,
  Red = 1,
  Yellow = 2
}

export interface IGame {
  Cells: number[][];

  CurrentState: GameState;
  YellowPlayerID: string;
  RedPlayerID: string;
  ID: string
}

interface IResponse<T> {
  ok: boolean;
  statusCode: number;
  body: T;
}

export default class {
  protocol: string;
  hostname: string;
  port: number;

  constructor() {
    this.protocol = 'http:';
    this.hostname = 'yorkdojoconnect4.azurewebsites.net';
    this.port = 80;
  }

  getGame(playerId: string): Promise<IResponse<IGame>> {
    const qs = querystring.stringify({ playerID: playerId });
    return this.do<IGame>('GET', `/api/GameState?${qs}`);
  }

  registerTeam(teamName: string, password: string): Promise<IResponse<string>> {
    const qs = querystring.stringify({ teamName: teamName, password: password });
    return this.do<string>('POST', `/api/Register?${qs}`);
  }

  makeMove(playerId: string, password: string, columnNumber: number): Promise<IResponse<void>> {
    const qs = querystring.stringify({ playerID: playerId, columnNumber: columnNumber, password: password });
    return this.do<void>('POST', `/api/MakeMove?${qs}`);
  }

  newGame(playerId: string): Promise<IResponse<void>> {
    const qs = querystring.stringify({ playerID: playerId });
    return this.do<void>('POST', `/api/NewGame?${qs}`);
  }

  private do<T>(method: string, path: string, data?: any): Promise<IResponse<T>> {
    const options = {
      method: method,
      protocol: this.protocol,
      port: this.port,
      hostname: this.hostname,
      path: path,
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      }
    };

    let buf;
    if (data) {
      buf = Buffer.from(JSON.stringify(data));
      options.headers['Content-Length'] = buf.length;
    }

    return new Promise((resolve, reject) => {
      let req = http.request(options, (res) => {
        if (res.statusCode < 200 || res.statusCode >= 300) {
          return resolve({ ok: false, statusCode: res.statusCode, body: null });
        }

        let chunks = new Array<Buffer>();
        res.on('data', (chunk) => chunks.push(chunk));
        res.on('end', () => resolve({
          ok: true,
          statusCode: res.statusCode,
          body: JSON.parse(Buffer.concat(chunks).toString('utf8'))
        }));
      });

      req.on('error', reject);

      if (data) {
        req.write(buf);
      }

      req.end();
    });
  }
}