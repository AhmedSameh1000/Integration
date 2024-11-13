import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ToDataBaseRoutingModule } from './to-data-base-routing.module';
import { ListdbtoComponent } from './listdbto/listdbto.component';


@NgModule({
  declarations: [
    ListdbtoComponent
  ],
  imports: [
    CommonModule,
    ToDataBaseRoutingModule
  ]
})
export class ToDataBaseModule { }
