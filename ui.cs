using Godot;
using System;
using BIGConnect.addons.godotblockchain;

public partial class ui : CanvasLayer
{
	bool isLoginStarted = false;
	
	[Export]
	private ERC20BlockchainContractNode				erc20ContractNode;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BlockchainClientNode.Instance.ClientLogMessage += DisplayLog;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public async void _on_button_pressed()
	{
		var emailEntry = GetNode<LineEdit>("/root/TestScene/CanvasLayer/VBoxContainer/EmailEntry");
		var otpEntry = GetNode<LineEdit>("/root/TestScene/CanvasLayer/VBoxContainer/OTPEntry");
		
		GD.Print("Testing");
		
		if ( isLoginStarted == false )
		{
			BlockchainClientNode.Instance.OnStartLogin( emailEntry.Text );	
			isLoginStarted = true;	
		}
		else
		{
			await BlockchainClientNode.Instance.OnOTPSubmit( otpEntry.Text );
		}
	}
	
	public void _on_smart_wallet_button_pressed()
	{
		BlockchainClientNode.Instance.CreateSmartWallet();
	}
	
	public async void _on_claim_erc20_button_pressed()
	{
		GD.Print("Pressed");
		
		erc20ContractNode.Initialize();
		
		if (erc20ContractNode != null)
		{
			ERC20BlockchainContractNode.ERC20TokenMetadata metadata = await erc20ContractNode.FetchMetadata();
			
			GD.Print(metadata.TokenName);
			
			await erc20ContractNode.Claim("1.0");
		}
		else
		{
			GD.Print("No contract node");
		}
	}
	
	public void DisplayLog( string message )
	{
		GD.Print( message );
	}
}
