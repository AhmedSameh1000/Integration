import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListdbtoComponent } from './listdbto/listdbto.component';

const routes: Routes = [
  {
    path: '',
    component: ListdbtoComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ToDataBaseRoutingModule {}
