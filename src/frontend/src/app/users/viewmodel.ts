
import { Subject } from 'rxjs';
import { GetUsersResponse, UserItem, UserRole } from '../api/reports/swagger';
import { Link } from '../ext/links';
import { NavigationLink } from '../navigation/ViewModel';

export class ViewModel
{
    Refresh(response: GetUsersResponse) {
      this.users = [];
      response.users.forEach(p => this.users.push(UserVm.FromApi(p)));
    }
    projectId: number;
    constructor(init?: Partial<ViewModel>) {
        Object.assign(this, init);
    }  
    selectedUser: UserVm;
    userRoles: UserRoleListItem[] = [ 
        new UserRoleListItem(UserRole.Guest),
        new UserRoleListItem(UserRole.Owner), 
        new UserRoleListItem(UserRole.TestManager), 
        new UserRoleListItem(UserRole.Tester) 
    ];
    users: UserVm[] = [];
    inviteMail: string;
    isAddUserModalVisible: boolean; 
    isInviteUserModalVisible: boolean; 
    isDeleteUserModalVisible: boolean; 
    isEditUserModalVisible: boolean;
    navigationLinks: NavigationLink[] = [];
}


class UserRoleListItem
{
    constructor(value: UserRole) 
    {
        this.value = value;
        this.name = UserRole[value];
    }
    name: string;
    value: UserRole;
}

 

export class UserVm  
{
    static FromApi(p: UserItem): UserVm {
        let reslt = new UserVm({
            id: p.id,
            login: p.login,
            mail: p.mail,
            name: p.name,
            role: p.role,
            userId: p.identity.id,
            isVirtual: p.isVirtual,
        });
        return reslt;
    }
    constructor(init?: Partial<UserVm>) 
    {
        Object.assign(this, init);
    }

    userId: number;
    id: number;
    name: string;
    login: string;
    pass: string;
    role: UserRole;
    isVirtual: boolean;
    get roleStr(): string
    {
        return UserRole[this.role];
    }
    mail: string;
} 

