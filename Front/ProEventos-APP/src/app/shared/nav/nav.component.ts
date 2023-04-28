import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '@app/Services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {

  isCollapsed = true; //vai começar o collapse com as informações fechadas

  constructor(private router: Router,
              public accountService: AccountService) { }

  ngOnInit(): void {
  }

  logout(): void{
    this.accountService.logout();
    this.router.navigateByUrl('/user/login')
  }

  showMenu(): boolean{
    return this.router.url !== '/user/login';
  }

}
