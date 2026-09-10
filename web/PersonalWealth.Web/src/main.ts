import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter, RouterOutlet, RouterLink } from '@angular/router';
import { Component } from '@angular/core';
import { routes } from './app/routes';
@Component({selector:'app-root',standalone:true,imports:[RouterOutlet,RouterLink],template:`<main><header><h1>Personal Wealth</h1><nav><a routerLink="/">Dashboard</a><a routerLink="/banking">Banking</a><a routerLink="/expenses">Expenses</a><a routerLink="/investments">Investments</a><a routerLink="/assets">Assets</a><a routerLink="/liabilities">Liabilities</a><a routerLink="/imports">Imports</a></nav></header><router-outlet/></main>`})
class AppComponent {}
bootstrapApplication(AppComponent,{providers:[provideHttpClient(),provideRouter(routes)]});
