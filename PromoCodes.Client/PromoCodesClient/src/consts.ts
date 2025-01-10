export class GenerateResponse {
    public result: boolean | undefined;
}

export class UseCodeResponse {
    public result: UseCodeResponseType | undefined;
}


export enum UseCodeResponseType {
    Success = 0,
    InvalidRequest = 1,
    CodeNotFound = 2
}

export const CodesGeneratedResponseMessage = "CodesGenerated";
export const CodeUsedResponseMessage = "CodeUsed";