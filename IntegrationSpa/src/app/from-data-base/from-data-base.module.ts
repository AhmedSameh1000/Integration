import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FromDataBaseRoutingModule } from './from-data-base-routing.module';
import { ListdbfromComponent } from './listdbfrom/listdbfrom.component';


@NgModule({
  declarations: [
    ListdbfromComponent
  ],
  imports: [
    CommonModule,
    FromDataBaseRoutingModule
  ]
})
export class FromDataBaseModule { }
