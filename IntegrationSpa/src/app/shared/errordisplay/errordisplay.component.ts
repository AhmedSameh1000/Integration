import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-errordisplay',
  templateUrl: './errordisplay.component.html',
  styleUrls: ['./errordisplay.component.css'],
})
export class ERRORDisplayComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: { err: string }) {}

  copyToClipboard(icon: HTMLElement) {
    icon.className = 'fa-solid fa-check';
    const textArea = document.createElement('textarea');
    textArea.value = this.data.err;
    document.body.appendChild(textArea);
    textArea.select();
    document.execCommand('copy');
    document.body.removeChild(textArea);

    setTimeout(() => {
      icon.className = 'fa-regular fa-clipboard';
    }, 1000);
  }
}
