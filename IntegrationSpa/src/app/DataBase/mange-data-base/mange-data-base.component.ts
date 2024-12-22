import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { DataBaseService } from 'src/app/Services/data-base.service';

@Component({
  selector: 'app-mange-data-base',
  templateUrl: './mange-data-base.component.html',
  styleUrls: ['./mange-data-base.component.css'],
})
export class MangeDataBaseComponent implements OnInit {
  constructor(
    private DbService: DataBaseService,
    private MatDilogRef: MatDialogRef<MangeDataBaseComponent>,
    private Toaster: ToastrService,
    @Inject(MAT_DIALOG_DATA) public data: { Id: any }
  ) {}
  ngOnInit(): void {
    this.InitilizeForm();
    this.GetDataBaseTypes();
    if (this.data) {
      this.GetDataBaseById();
    }
  }

  GetDataBaseById() {
    this.DbService.GetDataBaseById(this.data.Id).subscribe({
      next: (res: any) => {
        this.DataBaseModule.patchValue({
          Name: res.name,
          Connection: res.connection,
          DataBaseType: res.dataBaseType,
        });
      },
      error: (err) => {
        console.log(err);
      },
    });
  }
  DataBaseModule: FormGroup;

  InitilizeForm() {
    this.DataBaseModule = new FormGroup({
      Name: new FormControl(null, Validators.required),
      Connection: new FormControl(null, Validators.required),
      DataBaseType: new FormControl(null, Validators.required),
    });
  }
  DataBaseTypes: any;
  GetDataBaseTypes() {
    this.DbService.GetDataBaseTypes().subscribe({
      next: (res) => {
        console.log(res);
        this.DataBaseTypes = res;
      },
      error: (err) => {
        console.log(err);
      },
    });
  }
  AddDatabase() {
    if (this.DataBaseModule.invalid) return;
    this.DbService.AddDataBase(this.DataBaseModule.value).subscribe({
      next: (res) => {
        console.log(res);
        this.MatDilogRef.close(true);
        this.Toaster.success('Data Base Created Successfuly');
      },
      error: (err) => {
        console.log(err);
        this.Toaster.error('an error occurred while create database');
      },
    });
  }

  SaveDataBase() {
    if (this.DataBaseModule.invalid) {
      return;
    }
    var data = {
      Id: this.data.Id,
      Name: this.DataBaseModule.get('Name').value,
      Connection: this.DataBaseModule.get('Connection').value,
      DataBaseType: this.DataBaseModule.get('DataBaseType').value,
    };
    this.DbService.SaveDataBase(data).subscribe({
      next: (res) => {
        console.log(res);
        this.Toaster.success('Data Base Updated Successfuly');
        this.MatDilogRef.close(true);
      },
      error: (err) => {
        console.log(err);
      },
    });
  }
}
