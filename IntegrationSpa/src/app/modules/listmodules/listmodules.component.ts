import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { ModuleService } from 'src/app/Services/module.service';
import { MangeModuleComponent } from '../mange-module/mange-module.component';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-listmodules',
  templateUrl: './listmodules.component.html',
  styleUrls: ['./listmodules.component.css'],
})
export class ListmodulesComponent implements OnInit {
  Modules: any[] = [];

  constructor(
    private ModuleService: ModuleService,
    private toastr: ToastrService,
    private MatDilog: MatDialog
  ) {}

  ngOnInit(): void {
    this.GetModules();
    this.GetAutoValue();
  }

  AutoSyncValue: any;

  GetAutoValue() {
    this.ModuleService.GetAutoSyncvalue().subscribe({
      next: (res: any) => {
        this.AutoSyncValue = res.data;
        console.log(res);
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  async SyncAllModules() {
    var AutoSyncVaue = !this.AutoSyncValue;
    this.ModuleService.AutoSync(AutoSyncVaue).subscribe({
      next: (res: any) => {
        console.log(res);
        if (res.data) {
          this.AutoSyncValue = true; // Update AutoSyncValue after success
          this.toastr.success(res.message);
        } else {
          this.AutoSyncValue = false; // Update AutoSyncValue after failure
          this.toastr.info(res.message);
        }

        this.GetAutoValue();
      },
      error: (err) => {
        console.log(err);
        this.toastr.error(err.error.message);
      },
    });
  }

  GetModules() {
    this.ModuleService.GetModules().subscribe({
      next: (res: any[]) => {
        this.Modules = res.map((module) => ({ ...module, isLoading: false }));
        console.log(res);
      },
    });
  }

  syncModule(moduleId: number, syncType: any) {
    // العثور على العنصر الذي يتم مزامنته حاليا وتحديث حالة التحميل الخاصة به
    const module = this.Modules.find((mod) => mod.id === moduleId);
    if (module) {
      module.isLoading = true;
      console.log(`Syncing module with ID: ${moduleId}`);
      this.ModuleService.Sync(moduleId, syncType).subscribe({
        next: (res: any) => {
          console.log(res);
          module.isLoading = false; // إيقاف التحميل للعنصر المحدد فقط
          this.toastr.success(res.data + ' ' + res.message);
        },
        error: (err) => {
          console.log(err);
          module.isLoading = false; // إيقاف التحميل في حالة حدوث خطأ للعنصر المحدد فقط

          this.toastr.error(err.error.message);
        },
      });
    }
  }

  MangeModule() {
    const maxPriority =
      this.Modules.length > 0
        ? Math.max(...this.Modules.map((c) => c.priorty || 0)) // تجنب القيم undefined
        : 0;
    var Dilog = this.MatDilog.open(MangeModuleComponent, {
      width: '60%',
      disableClose: true,
      data: {
        maxpriorty: maxPriority + 1,
      },
    });
    Dilog.afterClosed().subscribe({
      next: (res) => {
        if (res) this.GetModules();
      },
    });
  }
  EditModule(Id) {
    var Dilog = this.MatDilog.open(MangeModuleComponent, {
      data: { Id: Id },
      disableClose: true,
    });
    Dilog.afterClosed().subscribe({
      next: (res) => {
        if (res) this.GetModules();
      },
    });
  }
  Delete(moduleid) {
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
        this.ModuleService.DeleteModule(moduleid).subscribe({
          next: (res) => {
            Swal.fire({
              title: 'Deleted!',
              text: 'Your file has been deleted.',
              icon: 'success',
            });
            this.GetModules();
          },
        });
      }
    });
  }
  disable(id) {
    Swal.fire({
      title: 'Are you sure?',
      text: 'You will disable this module',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, disable it!',
    }).then((result) => {
      if (result.isConfirmed) {
        this.ModuleService.disable(id).subscribe({
          next: (res: any) => {
            this.toastr.success(res.message);
            this.GetModules();
          },
          error: (err) => {
            this.toastr.error(err.error.message);
          },
        });
      }
    });
  }
  Enable(id) {
    Swal.fire({
      title: 'Are you sure?',
      text: 'You will enable this module',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, Enable it!',
    }).then((result) => {
      if (result.isConfirmed) {
        this.ModuleService.Enable(id).subscribe({
          next: (res: any) => {
            this.toastr.success(res.message);
            this.GetModules();
          },
          error: (err) => {
            this.toastr.error(err.error.message);
          },
        });
      }
    });
  }
}
