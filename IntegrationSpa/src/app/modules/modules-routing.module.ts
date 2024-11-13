import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListmodulesComponent } from './listmodules/listmodules.component';

const routes: Routes = [
  {
    path: '',
    component: ListmodulesComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ModulesRoutingModule {}
