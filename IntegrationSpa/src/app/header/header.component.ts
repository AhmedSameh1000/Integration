import { Component } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
})
export class HeaderComponent {
  constructor(public AuthService: AuthService, private Router: Router) {}
  LogOut() {
    this.AuthService.logout();
    this.Router.navigate(['/auth/login']);
  }
}
