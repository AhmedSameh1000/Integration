import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class ReferenceService {
  constructor(private HttpClient: HttpClient) {}

  AddReferance(ref) {
    return this.HttpClient.post(
      environment.BaseUrl + '/Reference/AddReference',
      ref
    );
  }
}
