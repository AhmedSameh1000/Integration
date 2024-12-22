import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { DataBaseService } from 'src/app/Services/data-base.service';

@Component({
  selector: 'app-mange-data-base-columns',
  templateUrl: './mange-data-base-columns.component.html',
  styleUrls: ['./mange-data-base-columns.component.css'],
})
export class MangeDataBaseColumnsComponent implements OnInit {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { dbId: any; dbType: any },
    private DataBaseService: DataBaseService,
    private MatDilogRef: MatDialogRef<MangeDataBaseColumnsComponent>,
    private Toaster: ToastrService
  ) {}
  ngOnInit(): void {
    this.InitializeForm();
    this.GetTables();
  }
  FormGroup: FormGroup;
  InitializeForm() {
    this.FormGroup = new FormGroup({
      Table: new FormControl(null, Validators.required),
      column: new FormControl(null, Validators.required),
      type: new FormControl(null, Validators.required),
      AllowNULL: new FormControl(false, Validators.required),
      primary: new FormControl(false, Validators.required),
    });
  }
  DataTypes = [
    { name: 'INT', value: 'INT' },
    { name: 'VARCHAR', value: 'VARCHAR(100)' },
    { name: 'DateTIME', value: 'DateTIME' },
    { name: 'BOOLEAN', value: 'BIT' },
    { name: 'TEXT', value: 'TEXT' },
    { name: 'DECIMAL', value: 'DECIMAL' },
  ];
  Tables: any;
  GetTables() {
    this.DataBaseService.GetTables(this.data.dbId).subscribe({
      next: (res) => {
        this.Tables = res;
      },
    });
  }
  saveConfiguration() {
    if (this.FormGroup.invalid) {
      return;
    }

    const Controls = this.FormGroup.controls;
    const table = Controls['Table'].value;
    const column = Controls['column'].value;
    const type = Controls['type'].value;
    const isPrimary = Controls['primary'].value;
    const allowNull = Controls['AllowNULL'].value ? 'NULL' : 'NOT NULL';

    let query = '';

    if (this.data.dbType === 'SqlServer') {
      query = `ALTER TABLE ${table} ADD ${column} ${type} ${allowNull}`;

      if (isPrimary) {
        query += ' IDENTITY(1,1) PRIMARY KEY';
      }
    } else {
      query = `ALTER TABLE ${table} ADD COLUMN ${column} ${
        type == 'BIT' ? 'BOOLEAN' : type
      } ${allowNull}`;

      if (isPrimary) {
        query += ' AUTO_INCREMENT PRIMARY KEY';
      }
    }
    query += ';';
    console.log(query);
    const options = {
      dbId: this.data.dbId,
      query: query,
    };

    this.DataBaseService.AddColumn(options).subscribe({
      next: (res: any) => {
        this.MatDilogRef.close(true);
        this.Toaster.success(res.message);
      },
      error: (err) => {
        this.Toaster.error(err.error.message);
        console.log(err);
      },
    });
  }
}
