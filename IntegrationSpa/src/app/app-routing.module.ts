import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    loadChildren: () =>
      import('./modules/modules.module').then((m) => m.ModulesModule),
  },
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.module').then((m) => m.AuthModule),
  },
  {
    path: 'fromdb',
    loadChildren: () =>
      import('./from-data-base/from-data-base.module').then(
        (m) => m.FromDataBaseModule
      ),
  },
  {
    path: 'todb',
    loadChildren: () =>
      import('./to-data-base/to-data-base.module').then(
        (m) => m.ToDataBaseModule
      ),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
