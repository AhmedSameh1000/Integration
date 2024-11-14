import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class DataBaseService {
  constructor(private httpClient: HttpClient) {}

  GetDataBases() {
    return this.httpClient.get(environment.BaseUrl + '/DataBase/list');
  }
  CheckConnection(ConStr, Type) {
    return this.httpClient.get(
      environment.BaseUrl +
        '/SqlDatabaseMetadata/check-connection?connectionString=' +
        ConStr +
        '&dataBaseType=' +
        Type
    );
  }
}
