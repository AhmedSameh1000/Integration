import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  Inject,
  signal,
  ViewChild,
} from '@angular/core';

import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatStepper } from '@angular/material/stepper';
import { ToastrService } from 'ngx-toastr';
import { DataBaseService } from 'src/app/Services/data-base.service';
import { ModuleService } from 'src/app/Services/module.service';
import { ReferenceService } from 'src/app/Services/reference.service';
import { StandardService } from 'src/app/Services/standard.service';
import { __values } from 'tslib';

@Component({
  selector: 'app-mange-module',
  templateUrl: './mange-module.component.html',
  styleUrls: ['./mange-module.component.css'],
})
export class MangeModuleComponent implements OES_element_index_uint {
  constructor(
    private manageModuleService: ModuleService,
    private toastr: ToastrService,
    private DataBaseService: DataBaseService,
    private MatdiloRef: MatDialogRef<MangeModuleComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { Id: string; maxpriorty: any },
    private Change: ChangeDetectorRef,
    private standardService: StandardService,
    private RefServices: ReferenceService
  ) {}
  standards: { control: string; statics: string[] }[] = [];

  Keys: any[] = [];
  ngOnInit(): void {
    this.GetStandards();

    this.initializeForm();
    this.GetDataBases();
    if (this.data.Id) {
      this.GetModuleById();
    }
  }

  generateRandomKey(length: number): string {
    const characters =
      'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < length; i++) {
      const randomIndex = Math.floor(Math.random() * characters.length);
      result += characters.charAt(randomIndex);
    }
    return '#' + result;
  }

  // Example usage:

  GetStandards() {
    this.DataBaseService.GetStandards().subscribe({
      next: (res: any) => {
        this.standards = res.map((item) => ({
          control: item.control, // Map Control to control
          statics: item.standards, // Map Standards to statics
        }));
      },
      error: (err) => {
        console.error('Error fetching standards', err);
      },
    });
  }

  setDefaultSelected(Columns: string[], isLocal: boolean) {
    if (!Columns || Columns.length === 0) return;

    const Controls = Object.keys(this.moduleForm.controls);

    for (const Control of Controls) {
      const standard = this.standards.find((std) => std.control === Control);

      if (standard) {
        const controlInForm = this.moduleForm.get(Control);

        const matchedColumn = Columns.find((Column) =>
          standard.statics.includes(Column.toLowerCase())
        );
        for (const column of Columns) {
        }

        if (
          matchedColumn &&
          !standard.statics.includes(controlInForm.value) &&
          (controlInForm.value != '' || controlInForm.value != null)
        ) {
          controlInForm.setValue(matchedColumn);
        }
      }
    }
  }

  isColumnMatching(column: string): boolean {
    return this.standardService.standards.some((standard) =>
      standard.statics.some(
        (stat) => stat.toLowerCase() === column.toLowerCase()
      )
    );
  }

  AddReferance(source: string, local: string, primary: string) {
    var Reference = new FormGroup({
      TableFromName: new FormControl(source, Validators.required),
      LocalName: new FormControl(local, Validators.required),
      PrimaryName: new FormControl(primary, Validators.required),
      key: new FormControl(),
    });

    if (Reference.invalid) {
      this.toastr.error('Error Invalid data');
      return;
    }
    if (this.data.Id) {
      var ref = {
        ModuleId: this.data.Id,
        TableFromName: Reference.get('TableFromName').value,
        LocalName: Reference.get('LocalName').value,
        PrimaryName: Reference.get('PrimaryName').value,
      };
      this.RefServices.AddReferance(ref).subscribe({
        next: (res: any) => {
          this.toastr.success(res.message);
        },
        error: (err) => {
          this.toastr.error(err.error.message);
        },
      });
    }

    // Push the form group to the References FormArray
    let References = this.moduleForm.get('References') as FormArray;
    References.push(Reference);
    setTimeout(() => {
      this.scrollToBottomRef();
    }, 0);
  }
  AddQuery(index, field, operator, value) {
    let Operations = this.moduleForm.get('Operations') as FormArray;

    if (!field || !operator || !value) return;
    let subquery = `${field} ${operator} '${value}'`;
    let operation = Operations.at(index) as FormGroup; // Get the operation form group at the specified index

    var CurrentValue = operation.get('OPCondition').value;
    if (
      CurrentValue == null ||
      CurrentValue == undefined ||
      CurrentValue == ''
    ) {
      operation.patchValue({
        OPCondition: 'Where ' + subquery + ' ',
      });
    } else {
      operation.patchValue({
        OPCondition: CurrentValue + subquery + ' ',
      });
    }
  }
  AddOperatorToOperationm(op, index) {
    let Operations = this.moduleForm.get('Operations') as FormArray;
    let operation = Operations.at(index) as FormGroup; // Get the operation form group at the specified index

    var CurrentValue = operation.get('OPCondition').value;

    if (CurrentValue == null || CurrentValue == undefined || CurrentValue == '')
      return;
    operation.patchValue({
      OPCondition: CurrentValue + op + ' ',
    });
  }
  ClearOpeQuery(index) {
    let Operations = this.moduleForm.get('Operations') as FormArray;
    let operation = Operations.at(index) as FormGroup; // Get the operation form group at the specified index
    if (operation.get('OPCondition')) {
      operation.patchValue({ OPCondition: '' }); // Reset the field to an empty string
    }
  }
  AddOperation(value) {
    debugger;
    if (value == 'null') {
      this.toastr.warning('Please select an type');
      return;
    }

    var CurrentOperation = this.moduleForm
      .get('Operations')
      .value.find((c) => c.Type == value);
    if (CurrentOperation != null) {
      this.toastr.warning('Cannot add multiple operations of the same type');
      return;
    }
    const Operation = new FormGroup({
      OPTableFromName: new FormControl(),
      OPTableToName: new FormControl(),
      OPToInsertFlag: new FormControl(),
      OPToUpdateFlag: new FormControl(),
      OPToDeleteFlag: new FormControl(),
      OPFromInsertFlag: new FormControl(),
      OPFromUpdateFlag: new FormControl(),
      OPFromDeleteFlag: new FormControl(),
      OPFromInsertDate: new FormControl(),
      OPFromItemIdName: new FormControl(),
      OPToItemLocalIdName: new FormControl(),
      OPFromItemPrice: new FormControl(),
      OPToItemPrice: new FormControl(),
      OPCondition: new FormControl(),
      LocalIdName: new FormControl(),
      CloudIdName: new FormControl(),
      OPToPrimary: new FormControl(),
      CustomerIdName: new FormControl(),
      StoreIdName: new FormControl(),
      OpToCustomerId: new FormControl(),
      OpToProductId: new FormControl(),
      OpCustomerReference: new FormControl(),
      OPProductReference: new FormControl(),
      OpFromPrimary: new FormControl(),
      OpInsertDate: new FormControl(),
      OpUpdateDate: new FormControl(),
      OPTOSellerPrimary: new FormControl(),
      OpSellerReference: new FormControl(),

      Type: new FormControl(value, Validators.required),
    });

    const Operations = this.moduleForm.get('Operations') as FormArray;
    Operations.push(Operation);
  }

  isDisabled = true;
  moduleForm: FormGroup;

  DataBases: any[] = []; // قائمة قواعد البيانات اللي هنعرضها في الـ select

  // جلب قواعد البيانات من الـ API
  GetDataBases() {
    this.DataBaseService.GetDataBases().subscribe({
      next: (res: any) => {
        this.DataBases = res; // تخزين بيانات قواعد البيانات في المتغير
      },
      error: (err) => {
        console.error('Error fetching databases:', err);
      },
    });
  }
  FromDataBaseSelected = false;
  ToDataBaseSelected = false;

  onSubmit() {
    // if (this.moduleForm.valid) {
    //   this.manageModuleService.saveModule(this.moduleForm.value).subscribe({
    //     next: (res) => {
    //       this.toastr.success('Module Saved Successfully');
    //       // Handle any additional logic like closing the dialog or navigating
    //     },
    //     error: (err) => {
    //       this.toastr.error('Failed to Save Module');
    //     }
    //   });
    // }
  }
  Fromtables: any;
  FromDataBaseSelectedId: any;
  OnDbFromChange(event) {
    var databaseId = event.target.value;
    this.FromDataBaseSelectedId = databaseId;
    this.DataBaseService.GetTables(databaseId).subscribe({
      next: (res) => {
        this.Fromtables = res;
      },
      error: (err) => {
        console.log(err);
      },
    });
  }
  ColumnsFrom: any;
  OnTableFromChange(event) {
    var TableName = event.target.value;
    this.DataBaseService.GetColumns(
      this.FromDataBaseSelectedId,
      TableName
    ).subscribe({
      next: (res: any) => {
        this.ColumnsFrom = res;
        if (!this.data.Id) {
          this.setDefaultSelected(res, true);
        }
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  Totables: any;
  ToDataBaseSelectedId: any;
  OnDbToChange(event) {
    var databaseId = event.target.value;
    this.ToDataBaseSelectedId = databaseId;
    this.DataBaseService.GetTables(databaseId).subscribe({
      next: (res) => {
        this.Totables = res;
      },
      error: (err) => {
        console.log(err);
      },
    });
  }
  ColumnsTo: any;
  OnTableChange(event) {
    var TableName = event.target.value;
    this.DataBaseService.GetColumns(
      this.ToDataBaseSelectedId,
      TableName
    ).subscribe({
      next: (res: any) => {
        this.ColumnsTo = res;
        if (!this.data.Id) {
          this.setDefaultSelected(res, false);
        }
      },
      error: (err) => {
        console.log(err);
      },
    });
  }
  selectedTabIndex = 0;

  nextTab() {
    if (this.selectedTabIndex >= 5) return;
    this.selectedTabIndex++;
  }
  Prevtab() {
    if (this.selectedTabIndex <= 0) return;

    this.selectedTabIndex--;
  }

  AddModule() {
    this.manageModuleService.CreateModule(this.moduleForm.value).subscribe({
      next: (res: any) => {
        this.toastr.success(res.message);
        this.MatdiloRef.close(true);
      },
      error: (err: any) => {
        this.toastr.error(err.error.message);

        console.log(err);
      },
    });
  }
  SaveModule() {
    var id = new FormControl(this.data.Id);
    this.moduleForm.addControl('id', id);
    this.manageModuleService.EditModule(this.moduleForm.value).subscribe({
      next: (res: any) => {
        this.toastr.success(res.message);
        this.MatdiloRef.close(true);
      },
      error: (err) => {
        console.log(err);
        this.toastr.error(err.error.message);
      },
    });
  }
  get Operations() {
    return this.moduleForm.get('Operations') as FormArray;
  }
  get Columns() {
    return this.moduleForm.get('Columns') as FormArray;
  }
  get Referance() {
    return this.moduleForm.get('References') as FormArray;
  }
  removeColumn(index: number) {
    this.Columns.removeAt(index);
  }

  removeReferance(index: number) {
    this.Referance.removeAt(index);
  }
  RemoveOperation(index) {
    this.Operations.removeAt(index);
  }
  selectedType: any;
  OnRefranceTableChange(
    event: Event,
    LocalSelected: HTMLSelectElement,
    primarySelect: HTMLSelectElement
  ) {
    const Table = (event.target as HTMLSelectElement).value;

    // Clear existing options for both LocalSelected and primarySelect
    LocalSelected.innerHTML = '';
    primarySelect.innerHTML = '';

    // Add default placeholder options
    const localOptionPlaceholder = document.createElement('option');
    localOptionPlaceholder.value = '';
    localOptionPlaceholder.text = 'Select Column';
    localOptionPlaceholder.disabled = true;
    localOptionPlaceholder.selected = true;

    const primaryOptionPlaceholder = document.createElement('option');
    primaryOptionPlaceholder.value = '';
    primaryOptionPlaceholder.text = 'Select Column';
    primaryOptionPlaceholder.disabled = true;
    primaryOptionPlaceholder.selected = true;

    LocalSelected.appendChild(localOptionPlaceholder);
    primarySelect.appendChild(primaryOptionPlaceholder);

    // Fetch columns and populate the select elements
    this.DataBaseService.GetColumns(this.ToDataBaseSelectedId, Table).subscribe(
      {
        next: (res: string[]) => {
          res.forEach((column) => {
            // Create and append options for LocalSelected
            const localOption = document.createElement('option');
            localOption.value = column;
            localOption.text = column;
            LocalSelected.appendChild(localOption);

            // Create and append options for primarySelect
            const primaryOption = document.createElement('option');
            primaryOption.value = column;
            primaryOption.text = column;
            primarySelect.appendChild(primaryOption);
          });
        },
        error: (err) => {
          console.error('Error fetching columns:', err);
          // Optionally, you can display an error message to the user
        },
      }
    );
  }

  OnModuleTypeChange(event) {
    var value = event.target.value;
    if (value == 0) this.ModuleType = 'Normal';
    else {
      this.ModuleType = 'Operations';
    }

    this.Change.detectChanges();
  }

  ModuleType: any = 'Normal';
  GetModuleById() {
    this.manageModuleService.GetModuleById(this.data.Id).subscribe({
      next: (res: any) => {
        console.log('Res here', res);
        this.ModuleType = res.data.syncType;
        this.ToDataBaseSelectedId = res.data.toDbId;
        this.FromDataBaseSelectedId = res.data.fromDbId;

        if (this.ModuleType == 'Normal') {
          this.GetInfo(res);
        }
        this.mapModuleToForm(res.data);

        if (this.ModuleType != 'Normal') {
          var Operations: any[] = res.data.operationForReturnDTOs;

          Operations.forEach((o) => {
            this.MapProductOperation(o);
            this.Change.detectChanges();
          });
        }

        this.Change.detectChanges(); // تحديث التمبليت
      },
    });
  }
  GetInfo(res) {
    this.DataBaseService.GetTables(this.FromDataBaseSelectedId).subscribe({
      next: (tablefrom: any) => {
        this.Fromtables = tablefrom;
      },
    });
    this.DataBaseService.GetTables(this.ToDataBaseSelectedId).subscribe({
      next: (tableto: any) => {
        this.Totables = tableto;
      },
    });

    this.DataBaseService.GetColumns(
      this.FromDataBaseSelectedId,
      res.data.tableFromName
    ).subscribe({
      next: (columnfrom) => {
        this.ColumnsFrom = columnfrom;
      },
    });
    this.DataBaseService.GetColumns(
      this.ToDataBaseSelectedId,
      res.data.tableToName
    ).subscribe({
      next: (ColumnTo) => {
        this.ColumnsTo = ColumnTo;
      },
    });
  }
  initializeForm() {
    this.moduleForm = new FormGroup({
      moduleName: new FormControl('', Validators.required),
      priority: new FormControl(this.data.maxpriorty, Validators.required),
      tableFromName: new FormControl(''),
      tableToName: new FormControl(''),
      toPrimaryKeyName: new FormControl(''),
      fromPrimaryKeyName: new FormControl(''),
      localIdName: new FormControl(''),
      cloudIdName: new FormControl(''),
      toDbId: new FormControl('', Validators.required),
      fromDbId: new FormControl('', Validators.required),
      toInsertFlagName: new FormControl(''),
      toUpdateFlagName: new FormControl(''),
      fromInsertFlagName: new FormControl(''),
      fromUpdateFlagName: new FormControl(''),
      fromDeleteFlagName: new FormControl(''),
      ToDeleteFlagName: new FormControl(''),
      SyncType: new FormControl(0),
      condition: new FormControl(null),
      Columns: new FormArray([]),
      References: new FormArray([]),
      Operations: new FormArray([]),
    });
  }
  mapModuleToForm(moduleData: any) {
    this.moduleForm = new FormGroup({
      moduleName: new FormControl(moduleData.name, Validators.required),
      priority: new FormControl(moduleData.priority, Validators.required),
      tableFromName: new FormControl(moduleData.tableFromName),
      fromDeleteFlagName: new FormControl(moduleData.fromDeleteFlagName),
      ToDeleteFlagName: new FormControl(moduleData.toDeleteFlagName),
      tableToName: new FormControl(moduleData.tableToName),
      condition: new FormControl(moduleData.condition),
      toPrimaryKeyName: new FormControl(moduleData.toPrimaryKeyName),
      fromPrimaryKeyName: new FormControl(moduleData.fromPrimaryKeyName),
      localIdName: new FormControl(moduleData.localIdName),
      cloudIdName: new FormControl(moduleData.cloudIdName),
      toDbId: new FormControl(moduleData.toDbId, Validators.required),
      fromDbId: new FormControl(moduleData.fromDbId, Validators.required),
      toInsertFlagName: new FormControl(moduleData.toInsertFlagName),
      toUpdateFlagName: new FormControl(moduleData.toUpdateFlagName),
      fromInsertFlagName: new FormControl(moduleData.fromInsertFlagName),
      fromUpdateFlagName: new FormControl(moduleData.fromUpdateFlagName),
      Columns: new FormArray([]),
      References: new FormArray([]),
      Operations: new FormArray([]),
    });
    const columnsArray = this.moduleForm.get('Columns') as FormArray;
    moduleData.columnsFromDTOs.forEach((column: any) => {
      columnsArray.push(
        new FormGroup({
          ColumnFrom: new FormControl(
            column.columnFromName,
            Validators.required
          ),
          ColumnTo: new FormControl(column.columnToName, Validators.required),
          IsReference: new FormControl(column.isReference, Validators.required),
          key: new FormControl(column.key, Validators.required),
          Referance: new FormControl(column.tableReferenceName),
        })
      );
    });

    this.GetColumnsReferance(moduleData.referancesForReturnDTOs);
  }

  Element: any;
  GetColumnsReferance(referancesForReturnDTOs) {
    const Referances = this.moduleForm.get('References') as FormArray;

    referancesForReturnDTOs.forEach((column: any) => {
      Referances.push(
        new FormGroup({
          TableFromName: new FormControl(column.tableFromName),
          LocalName: new FormControl(column.localName),
          PrimaryName: new FormControl(column.primaryName),
          key: new FormControl(column.key),
        })
      );
    });
  }
  @ViewChild('map') scrollableDiv!: ElementRef;
  @ViewChild('refscrol') refscrol!: ElementRef;
  scrollToBottom() {
    const div = this.scrollableDiv.nativeElement;
    div.scrollTop = div.scrollHeight;
  }
  scrollToBottomRef() {
    const div = this.refscrol.nativeElement;
    div.scrollTop = div.scrollHeight;
  }

  MapProductOperation(ProductOperation) {
    const Operations = this.moduleForm.get('Operations') as FormArray;

    this.DataBaseService.GetTables(this.ToDataBaseSelectedId).subscribe({
      next: (res) => {
        this.Totables = res;
      },
    });
    this.DataBaseService.GetTables(this.FromDataBaseSelectedId).subscribe({
      next: (res) => {
        this.Fromtables = res;
      },
    });

    this.DataBaseService.GetColumns(
      this.ToDataBaseSelectedId,
      ProductOperation.tableTo
    ).subscribe({
      next: (res) => {
        this.OperationToColumns = res;
      },
    });
    this.DataBaseService.GetColumns(
      this.FromDataBaseSelectedId,
      ProductOperation.tableFrom
    ).subscribe({
      next: (res) => {
        this.OperationFromTColumns = res;
      },
    });

    const Operation = new FormGroup({
      OPTableFromName: new FormControl(ProductOperation.tableFrom),
      OPTOSellerPrimary: new FormControl(ProductOperation.optoSellerPrimary),
      OPTableToName: new FormControl(ProductOperation.tableTo),
      OPToInsertFlag: new FormControl(ProductOperation.toInsertFlag),
      OPToUpdateFlag: new FormControl(ProductOperation.toUpdateFlag),
      OPToDeleteFlag: new FormControl(ProductOperation.toDeleteFlag),
      OPFromInsertFlag: new FormControl(ProductOperation.fromInsertFlag),
      OPFromUpdateFlag: new FormControl(ProductOperation.fromUpdateFlag),
      OPFromDeleteFlag: new FormControl(ProductOperation.fromDeleteFlag),
      OPFromInsertDate: new FormControl(ProductOperation.fromInsertDate),
      OPFromItemIdName: new FormControl(ProductOperation.itemId),
      OPToItemLocalIdName: new FormControl(ProductOperation.localId),
      OPFromItemPrice: new FormControl(ProductOperation.priceFrom),
      OPToItemPrice: new FormControl(ProductOperation.priceTo),
      OPCondition: new FormControl(ProductOperation.condition),
      LocalIdName: new FormControl(ProductOperation.localId),
      CloudIdName: new FormControl(ProductOperation.cloudId),
      OPToPrimary: new FormControl(ProductOperation.opToPrimary),
      CustomerIdName: new FormControl(ProductOperation.customerId),
      StoreIdName: new FormControl(ProductOperation.storeId),
      OpToCustomerId: new FormControl(ProductOperation.opToCustomerId),
      OpToProductId: new FormControl(ProductOperation.opToProductId),
      OpFromPrimary: new FormControl(ProductOperation.opFromPrimary),
      OpInsertDate: new FormControl(ProductOperation.opInsertDate),
      OpUpdateDate: new FormControl(ProductOperation.opUpdateDate),
      OpSellerReference: new FormControl(ProductOperation.opSellerReference),
      OpCustomerReference: new FormControl(
        ProductOperation.opCustomerReference
      ),
      OPProductReference: new FormControl(ProductOperation.opProductReference),
      Type: new FormControl(
        ProductOperation.operationType,
        Validators.required
      ),
    });
    Operations.push(Operation);
  }
  AddColumn() {
    var Column = new FormGroup({
      ColumnTo: new FormControl(null),
      ColumnFrom: new FormControl(null),
      key: new FormControl(),
    });

    let Columns = this.moduleForm.get('Columns') as FormArray;

    Columns.push(Column);
    setTimeout(() => {
      this.scrollToBottom();
    }, 0);
  }
  readonly panelOpenState = signal(false);

  addCondition(
    field: string,
    operator: string,
    value: string,
    btwval2: string
  ) {
    if (!field || !operator || !value) return;

    var conditionvalue: string = this.moduleForm.get('condition').value;

    // إذا كانت القيمة فارغة، تبدأ بـ "Where"
    if (conditionvalue === '') conditionvalue += 'Where ';

    let subquery;
    if (operator === 'BETWEEN' && btwval2) {
      // إذا كانت العملية بين وبين، تأكد من وجود قيمة الثانية
      subquery = `${field} ${operator} '${value}' AND '${btwval2}'`;
    } else {
      subquery = `${field} ${operator} '${value}'`;
    }

    conditionvalue += subquery;

    this.moduleForm.patchValue({
      condition: conditionvalue,
    });
  }

  addOperator(operator: string) {
    var condition = this.moduleForm.get('condition').value;

    if (condition) {
      // تأكد من أنه يوجد شرط سابق لإضافة المشغل
      condition += ` ${operator} `;
      // تحديث القيمة في النموذج
      this.moduleForm.patchValue({
        condition: condition,
      });
    }
  }

  clearQuery() {
    this.moduleForm.get('condition').reset();
  }

  BetweenDisplay = false;
  ShowBetweenValue(operator: string) {
    this.BetweenDisplay = operator === 'BETWEEN';
  }
  Operators = [
    '=',
    '!=',
    '>',
    '<',
    '>=',
    '<=',
    'BETWEEN',
    'In',
    'NOT IN',
    'IS NULL',
    'IS NOT NULL',
    'NOT',
  ];
  Types = [
    { type: 0, name: 'Normal', Selected: true },
    { type: 1, name: 'Operation', Selected: false },
  ];
  OperationFromTColumns: any;
  FromOperationChange(tabelFrom) {
    var table = tabelFrom.target.value;
    this.DataBaseService.GetColumns(
      this.FromDataBaseSelectedId,
      table
    ).subscribe({
      next: (res) => {
        this.OperationFromTColumns = res;
      },
    });
  }
  OperationToColumns: any;

  ToOperationChange(tabelto) {
    var table = tabelto.target.value;
    this.DataBaseService.GetColumns(this.ToDataBaseSelectedId, table).subscribe(
      {
        next: (res) => {
          this.OperationToColumns = res;
        },
      }
    );
  }
}
