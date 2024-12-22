import { Component, ElementRef, OnInit, Renderer2 } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DataBaseService } from 'src/app/Services/data-base.service';
import { MangeDataBaseComponent } from '../mange-data-base/mange-data-base.component';
import Swal from 'sweetalert2';
import { MangeDataBaseColumnsComponent } from '../mange-data-base-columns/mange-data-base-columns.component';

@Component({
  selector: 'app-listdatabases',
  templateUrl: './listdatabases.component.html',
  styleUrls: ['./listdatabases.component.css'],
})
export class ListdatabasesComponent implements OnInit {
  constructor(
    private DataBaseService: DataBaseService,
    private MatDilog: MatDialog,
    private renderer: Renderer2
  ) {}

  DataBases: any[] = []; // Make sure this matches your data structure
  resizeListener!: () => void;
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
    this.DataBaseService.CheckConnection(element.id).subscribe({
      next: (res) => {
        console.log(res);
        Icon.className = 'fa-solid fa-check';
      },
      error: (err) => {
        console.log(err);

        Icon.className = 'fa-solid fa-x';
        btn.color = 'warn';
      },
    });
  }
  OpenModuleForCreateDataBase() {
    var Dilog = this.MatDilog.open(MangeDataBaseComponent, {
      width: '50%',
    });

    Dilog.afterClosed().subscribe({
      next: (res) => {
        if (res) this.GetDataBases();
      },
    });
  }
  Edit(db) {
    console.log(db);
    var Dilog = this.MatDilog.open(MangeDataBaseComponent, {
      data: { Id: db.id },
      width: '50%',
    });

    Dilog.afterClosed().subscribe({
      next: (res) => {
        if (res) this.GetDataBases();
      },
    });
  }
  Delete(dbid) {
    Swal.fire({
      title: 'Are you sure?',
      text: "You won't be able to revert this!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, delete it!',
    }).then((result) => {
      if (result.isConfirmed) {
        this.DataBaseService.DeleteDataBase(dbid).subscribe({
          next: (res) => {
            Swal.fire({
              title: 'Deleted!',
              text: 'DataBase has been deleted.',
              icon: 'success',
            });
            this.GetDataBases();
          },
          error: (err) => {
            console.log(err);
            Swal.fire({
              title: 'Error!',
              text: err.error.message,
              icon: 'error',
            });
          },
        });
      }
    });
  }

  MangeDataBase(dbid, dbType) {
    this.MatDilog.open(MangeDataBaseColumnsComponent, {
      data: { dbId: dbid, dbType },
      width: '50%',
    });
  }
}
