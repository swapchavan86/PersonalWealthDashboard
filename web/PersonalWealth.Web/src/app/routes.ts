import { Routes } from '@angular/router';
import { DashboardPage,BankingPage,ExpensesPage,InvestmentsPage,AssetsPage,LiabilitiesPage,ImportsPage } from './pages';
export const routes: Routes = [
 {path:'',component:DashboardPage},{path:'banking',component:BankingPage},{path:'expenses',component:ExpensesPage},{path:'investments',component:InvestmentsPage},{path:'assets',component:AssetsPage},{path:'liabilities',component:LiabilitiesPage},{path:'imports',component:ImportsPage},{path:'**',redirectTo:''}
];
