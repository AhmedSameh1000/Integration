import { Component, ElementRef, OnInit } from '@angular/core';
import { DataBaseService } from 'src/app/Services/data-base.service';

@Component({
  selector: 'app-listdatabases',
  templateUrl: './listdatabases.component.html',
  styleUrls: ['./listdatabases.component.css'],
})
export class ListdatabasesComponent implements OnInit {
  constructor(private DataBaseService: DataBaseService) {}

  DataBases: any[] = []; // Make sure this matches your data structure

  ngOnInit(): void {
    this.GetDataBases();
  }

  GetDataBases() {
    this.DataBaseService.GetDataBases().subscribe({
      next: (res: any) => {
        this.DataBases = res;
        console.log(this.DataBases);
      },
      error: (err) => {
        console.error('Error fetching databases:', err);
      },
    });
  }
  checkConnection(element, Icon: HTMLElement, btn: any) {
    console.log(element);
    this.DataBaseService.CheckConnection(
      element.connection,
      element.dataBaseType
    ).subscribe({
      next: (res) => {
        Icon.className = 'fa-solid fa-check';
      },
      error: (err) => {
        Icon.className = 'fa-solid fa-x';
        btn.color = 'warn';
      },
    });
  }
}
