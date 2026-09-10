import {Injectable} from '@angular/core';
@Injectable({providedIn:'root'}) export class AuthService {private token='';setToken(value:string){this.token=value;}clear(){this.token='';}getToken(){return this.token;}isAuthenticated(){return this.token.length>0;}}
