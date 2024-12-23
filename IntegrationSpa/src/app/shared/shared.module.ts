import { NgModule } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatSliderModule } from '@angular/material/slider';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MatOptionModule } from '@angular/material/core';
import { MatRadioModule } from '@angular/material/radio';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatTabsModule } from '@angular/material/tabs';
import { MatExpansionModule } from '@angular/material/expansion';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { CommonModule, NgIf } from '@angular/common';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatStepperModule } from '@angular/material/stepper';
import { ERRORDisplayComponent } from './errordisplay/errordisplay.component';

let materialmodule = [
  MatInputModule,
  MatSelectModule,
  MatExpansionModule,
  MatAutocompleteModule,
  MatToolbarModule,
  MatMenuModule,
  MatStepperModule,
  MatIconModule,
  MatButtonModule,
  MatBadgeModule,

  MatSidenavModule,
  MatListModule,
  MatCardModule,
  MatSliderModule,
  MatTableModule,
  MatPaginatorModule,
  MatSortModule,
  MatDatepickerModule,
  MatNativeDateModule,
  MatRadioModule,
  MatCheckboxModule,
  MatDialogModule,
  MatGridListModule,
  MatTabsModule,
  MatIconModule,
  MatMenuModule,
  FormsModule,
  ReactiveFormsModule,
  CommonModule,
  MatProgressSpinnerModule,
  NgIf,
  DragDropModule,
  MatSlideToggleModule,
];
@NgModule({
  imports: [materialmodule],
  exports: [materialmodule, ERRORDisplayComponent],
  declarations: [ERRORDisplayComponent],
})
export class SharedModule {}
