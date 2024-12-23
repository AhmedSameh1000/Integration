import { Component } from '@angular/core';
import { AuthService } from './Services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  title = 'IntegrationSpa';
  constructor(public AuthService: AuthService) {}
}
//npm run electron-build
//electron-packager ./ IntegrationSpa --platform=win32 --overwrite
