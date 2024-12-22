import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class DataBaseService {
  constructor(private httpClient: HttpClient) {}

  GetDataBases() {
    return this.httpClient.get(environment.BaseUrl + '/DataBase/DataBases');
  }

  GetDataBaseById(id) {
    return this.httpClient.get(
      environment.BaseUrl + '/DataBase/GetDataBase/' + id
    );
  }
  CheckConnection(databaseId) {
    return this.httpClient.get(
      environment.BaseUrl +
        '/DataBaseMetaData/check-connection?DataBaseId=' +
        databaseId
    );
  }

  GetTables(databaseId) {
    return this.httpClient.get(
      environment.BaseUrl + '/DataBaseMetaData/tables?DataBaseId=' + databaseId
    );
  }
  GetColumns(databaseId, tableName) {
    return this.httpClient.get(
      environment.BaseUrl +
        '/DataBaseMetaData/columns?DataBaseId=' +
        databaseId +
        '&tableName=' +
        tableName
    );
  }
  GetDataBaseTypes() {
    return this.httpClient.get(
      environment.BaseUrl + '/DataBase/GetDataBaseTypes'
    );
  }
  AddDataBase(database) {
    return this.httpClient.post(
      environment.BaseUrl + '/DataBase/addDataBase',
      database
    );
  }
  SaveDataBase(database) {
    return this.httpClient.put(
      environment.BaseUrl + '/DataBase/editDataBase',
      database
    );
  }
  DeleteDataBase(dbId) {
    return this.httpClient.delete(
      environment.BaseUrl + '/DataBase/DeleteDataBase/' + dbId
    );
  }
  AddColumn(Options) {
    return this.httpClient.post(
      environment.BaseUrl + '/DataBase/AddColumn/',
      Options
    );
  }
  GetStandards() {
    return this.httpClient.get(
      environment.BaseUrl + '/DataBaseMetaData/GetStatics'
    );
  }
}
